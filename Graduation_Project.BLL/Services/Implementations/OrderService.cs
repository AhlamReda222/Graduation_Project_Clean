using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Order;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<OrderDto>> CreateOrderFromCartAsync(int userId, CreateOrderDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. جيب الكارت
                var cartItems = await _unitOfWork.CartItems
                    .GetQueryable()
                    .Include(c => c.Product)
                    .Include(c => c.ProductVariant)
                    .Include(c => c.Technique)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return ServiceResult<OrderDto>.Failure("Cart is empty");

                // 2. Validation + حساب الإجمالي
                decimal totalAmount = 0;

                foreach (var item in cartItems)
                {
                    if (item.Product == null || !item.Product.IsActive)
                        return ServiceResult<OrderDto>.Failure(
                            $"Product '{item.Product?.ProductName}' is no longer available");

                    var unitPrice = item.ProductVariant?.Price ?? item.Product.BasePrice;
                    var custPrice = item.Technique?.Price ?? 0;

                    if (item.ProductVariant != null && item.ProductVariant.StockQuantity < item.Quantity)
                        return ServiceResult<OrderDto>.Failure(
                            $"Not enough stock for '{item.Product.ProductName}' - only {item.ProductVariant.StockQuantity} available");

                    totalAmount += (unitPrice + custPrice) * item.Quantity;
                }

                // 3. Validate الدفع قبل ما نعمل الأوردر
                var paymentValidation = ValidatePayment(dto);
                if (!paymentValidation.IsSuccess)
                    return ServiceResult<OrderDto>.Failure(paymentValidation.Message);

                // 4. Simulate الدفع
                var paymentResult = await SimulatePaymentAsync(dto, totalAmount);
                if (!paymentResult.IsSuccess)
                    return ServiceResult<OrderDto>.Failure(paymentResult.Message);

                // 5. اعمل الـ Order
                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    OrderStatus = OrderStatus.Processing, // مباشرة Confirmed بعد الدفع
                    ShippingAddress = $"{dto.FirstName} {dto.LastName} - {dto.ShippingAddress}",
                    PaymentMethod = dto.PaymentMethod,
                    PaymentStatus = PaymentStatus.Paid,
                    TrackingNumber = GenerateTrackingNumber(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveAsync();

                // 6. اعمل الـ OrderItems
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cartItems)
                {
                    var unitPrice = cartItem.ProductVariant?.Price ?? cartItem.Product.BasePrice;
                    var custPrice = cartItem.Technique?.Price ?? 0;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        VariantId = cartItem.VariantId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = unitPrice,
                        CustomizationPrice = custPrice,
                        Subtotal = (unitPrice + custPrice) * cartItem.Quantity
                    };

                    orderItems.Add(orderItem);
                    await _unitOfWork.OrderItems.AddAsync(orderItem);
                }

                await _unitOfWork.SaveAsync();

                // 7. Customization
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.TechniqueId != null && cartItem.CustomizationZone != null)
                    {
                        var orderItem = orderItems.First(o =>
                            o.ProductId == cartItem.ProductId &&
                            o.VariantId == cartItem.VariantId);

                        await _unitOfWork.OrderItemCustomizations.AddAsync(new OrderItemCustomization
                        {
                            OrderItemId = orderItem.OrderItemId,
                            Zone = (CustomizationZone)cartItem.CustomizationZone.Value,
                            TechniqueId = cartItem.TechniqueId.Value,
                            DesignImageUrl = cartItem.DesignImageUrl,
                            DesignText = cartItem.DesignText,
                            CustomizationPrice = cartItem.Technique?.Price ?? 0
                        });
                    }
                }

                // 8. نقص الـ Stock
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.ProductVariant != null)
                    {
                        cartItem.ProductVariant.StockQuantity -= cartItem.Quantity;
                        _unitOfWork.ProductVariants.Update(cartItem.ProductVariant);
                    }
                    else
                    {
                        cartItem.Product.StockQuantity -= cartItem.Quantity;
                        _unitOfWork.Products.Update(cartItem.Product);
                    }
                }

                // 9. امسح الكارت
                foreach (var cartItem in cartItems)
                    _unitOfWork.CartItems.Delete(cartItem);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                // 10. رجع الـ Order مع كل التفاصيل
                var result = await GetOrderByIdAsync(order.OrderId, userId);
                if (result.Succeeded)
                {
                    result.Data.TransactionId = paymentResult.TransactionId;
                    result.Data.PaymentMessage = paymentResult.Message;
                }

                return result;
            }
          catch (Exception ex)
{
    await transaction.RollbackAsync();
    // ✅ بيجيب الـ inner exception كامل
    var fullError = ex.InnerException?.Message ?? ex.Message;
    return ServiceResult<OrderDto>.Failure($"Error: {fullError}");
}
        }

        public async Task<ServiceResult<OrderDto>> GetOrderByIdAsync(int orderId, int userId)
        {
            try
            {
                var order = await _unitOfWork.Orders
                    .GetQueryable()
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Brand)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Customizations)
                            .ThenInclude(c => c.Technique)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

                if (order == null)
                    return ServiceResult<OrderDto>.Failure("Order not found");

                return ServiceResult<OrderDto>.Success(MapToDto(order));
            }
            catch (Exception ex)
            {
                return ServiceResult<OrderDto>.Failure($"Error fetching order: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<OrderDto>>> GetMyOrdersAsync(int userId)
        {
            try
            {
                var orders = await _unitOfWork.Orders
                    .GetQueryable()
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Brand)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Customizations)
                            .ThenInclude(c => c.Technique)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<OrderDto>>.Success(orders.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<OrderDto>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<OrderDto>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders
                    .GetQueryable()
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Brand)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Customizations)
                            .ThenInclude(c => c.Technique)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<OrderDto>>.Success(orders.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<OrderDto>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<OrderDto>> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _unitOfWork.Orders
                    .GetQueryable()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    return ServiceResult<OrderDto>.Failure("Order not found");

                order.OrderStatus = status;
                order.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();

                return await GetOrderByIdAsync(orderId, order.UserId);
            }
            catch (Exception ex)
            {
                return ServiceResult<OrderDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<OrderDto>> CancelOrderAsync(int orderId, int userId)
        {
            try
            {
                var order = await _unitOfWork.Orders
                    .GetQueryable()
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

                if (order == null)
                    return ServiceResult<OrderDto>.Failure("Order not found");

                if (order.OrderStatus != OrderStatus.Pending &&
                    order.OrderStatus != OrderStatus.Processing)
                    return ServiceResult<OrderDto>.Failure("This order cannot be cancelled");

                order.OrderStatus = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                // رجّع الـ Stock
                foreach (var item in order.OrderItems)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity += item.Quantity;
                        _unitOfWork.ProductVariants.Update(item.ProductVariant);
                    }
                }

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();

                return await GetOrderByIdAsync(orderId, userId);
            }
            catch (Exception ex)
            {
                return ServiceResult<OrderDto>.Failure($"Error: {ex.Message}");
            }
        }

        // ✅ Validate الدفع قبل ما نبدأ
        private (bool IsSuccess, string Message) ValidatePayment(CreateOrderDto dto)
        {
            if (dto.PaymentMethod == PaymentMethod.CreditCard)
            {
                if (dto.CreditCard == null)
                    return (false, "Credit card details are required");

                if (string.IsNullOrEmpty(dto.CreditCard.CardNumber) ||
                    dto.CreditCard.CardNumber.Length != 16)
                    return (false, "Invalid card number");

                if (!IsValidExpiryDate(dto.CreditCard.ExpiryDate))
                    return (false, "Card has expired or invalid expiry date");

                if (string.IsNullOrEmpty(dto.CreditCard.CVC) ||
                    dto.CreditCard.CVC.Length != 3)
                    return (false, "Invalid CVC");

                if (string.IsNullOrEmpty(dto.CreditCard.CardHolderName))
                    return (false, "Card holder name is required");
            }

            return (true, "Valid");
        }

        // ✅ Simulate الدفع
        private async Task<(bool IsSuccess, string Message, string TransactionId)>
            SimulatePaymentAsync(CreateOrderDto dto, decimal amount)
        {
            await Task.Delay(1000); // Simulate gateway delay

            if (dto.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                return (
                    true,
                    "Order confirmed. Pay when your order arrives.",
                    $"COD-{Guid.NewGuid().ToString("N")[..10].ToUpper()}"
                );
            }

            // Credit Card Simulation
            var card = dto.CreditCard!;

            // بطاقة تبدأ بـ 0000 = فاشلة (للاختبار)
            if (card.CardNumber.StartsWith("0000"))
                return (false, "Payment declined by bank", null);

            return (
                true,
                $"Payment of {amount} EGP processed successfully",
                $"TXN-{Guid.NewGuid().ToString("N")[..12].ToUpper()}"
            );
        }

        private bool IsValidExpiryDate(string expiry)
        {
            if (string.IsNullOrEmpty(expiry) || !expiry.Contains('/'))
                return false;

            var parts = expiry.Split('/');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out int month) ||
                !int.TryParse(parts[1], out int year))
                return false;

            var expiryDate = new DateTime(2000 + year, month, 1)
                .AddMonths(1).AddDays(-1);

            return expiryDate >= DateTime.UtcNow;
        }

        private string GenerateTrackingNumber()
            => $"LB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        private OrderDto MapToDto(Order order) => new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            CustomerName = order.User?.FullName,
            TotalAmount = order.TotalAmount,
            ShippingCost = 0,
            FinalTotal = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            OrderStatusText = order.OrderStatus.ToString(),
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            PaymentMethodText = order.PaymentMethod.ToString(),
            PaymentStatus = order.PaymentStatus,
            PaymentStatusText = order.PaymentStatus.ToString(),
            TrackingNumber = order.TrackingNumber,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems?.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.ProductName,
                ProductImage = oi.Product?.ImageUrls?.Split(',').FirstOrDefault()?.Trim(),
                BrandName = oi.Product?.Brand?.BrandName,
                VariantId = oi.VariantId,
                Size = oi.ProductVariant?.Size,
                Color = oi.ProductVariant?.Color,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                CustomizationPrice = oi.CustomizationPrice,
                Subtotal = oi.Subtotal,
                Customization = oi.Customizations?.FirstOrDefault() != null
                    ? new OrderItemCustomizationDto
                    {
                        Zone = oi.Customizations.First().Zone.ToString(),
                        TechniqueName = oi.Customizations.First().Technique?.Name,
                        DesignImageUrl = oi.Customizations.First().DesignImageUrl,
                        DesignText = oi.Customizations.First().DesignText,
                        TechniquePrice = oi.Customizations.First().Technique?.Price ?? 0
                    }
                    : null
            }).ToList() ?? new()
        };
    }
}