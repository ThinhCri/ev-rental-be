# 💳 Hướng dẫn cấu hình VNPay Production

## 🎯 **Có thể deploy mà không cần VNPay sandbox!**

Bạn có tài khoản merchant VNPay rồi, tôi sẽ hướng dẫn cách cấu hình cho production.

## 🔧 **Cấu hình VNPay Production:**

### **Bước 1: Lấy thông tin từ VNPay Merchant**

Từ tài khoản merchant VNPay của bạn, lấy:
- **Terminal ID (TmnCode)**
- **Hash Secret**
- **Return URL** (URL callback sau khi thanh toán)

### **Bước 2: Cập nhật Environment Variables trên Render**

Vào Web Service → Environment Variables, thêm:

```
VnPay__TmnCode=YOUR_TERMINAL_ID
VnPay__HashSecret=YOUR_HASH_SECRET
VnPay__Url=https://payment.vnpayment.vn/vpcpay.html
VnPay__ReturnUrl=https://ev-rental-be-2.onrender.com/payment/return
VnPay__Command=pay
VnPay__CurrCode=VND
VnPay__Version=2.1.0
VnPay__Locale=vn
```

### **Bước 3: Cập nhật appsettings.Production.json**

```json
{
  "VnPay": {
    "TmnCode": "YOUR_TERMINAL_ID",
    "HashSecret": "YOUR_HASH_SECRET", 
    "Url": "https://payment.vnpayment.vn/vpcpay.html",
    "ReturnUrl": "https://ev-rental-be-2.onrender.com/payment/return",
    "Command": "pay",
    "CurrCode": "VND",
    "Version": "2.1.0",
    "Locale": "vn"
  }
}
```

## 🚀 **Cách deploy không cần VNPay sandbox:**

### **Option 1: Deploy với VNPay Production (Khuyến nghị)**
1. **Lấy thông tin từ merchant account VNPay**
2. **Cập nhật Environment Variables trên Render**
3. **Deploy bình thường**

### **Option 2: Deploy với Mock Payment (Cho test)**
1. **Tạm thời disable VNPay**
2. **Sử dụng mock payment response**
3. **Test booking flow**

### **Option 3: Deploy với VNPay Sandbox (Tạm thời)**
1. **Sử dụng sandbox credentials**
2. **Test với tiền ảo**
3. **Sau đó chuyển sang production**

## 🔧 **Cấu hình Mock Payment (Cho test):**

Nếu muốn test mà không cần VNPay thật:

```json
{
  "VnPay": {
    "TmnCode": "MOCK_TMN",
    "HashSecret": "MOCK_SECRET",
    "Url": "https://ev-rental-be-2.onrender.com/payment/mock",
    "ReturnUrl": "https://ev-rental-be-2.onrender.com/payment/return",
    "Command": "pay",
    "CurrCode": "VND",
    "Version": "2.1.0",
    "Locale": "vn"
  }
}
```

## 📋 **Checklist deploy:**

- [ ] ✅ Có tài khoản merchant VNPay
- [ ] ✅ Lấy Terminal ID và Hash Secret
- [ ] ✅ Cập nhật Environment Variables
- [ ] ✅ Cập nhật Return URL
- [ ] ✅ Test payment flow
- [ ] ✅ Deploy lên Render

## 🎯 **Kết quả:**

Sau khi deploy:
- ✅ Booking flow hoạt động
- ✅ Tạo QR code thanh toán
- ✅ Redirect đến VNPay
- ✅ Xử lý callback từ VNPay
- ✅ Cập nhật trạng thái đơn hàng

## 💡 **Lưu ý:**

1. **Return URL:** Phải là URL thật của app trên Render
2. **Hash Secret:** Giữ bí mật, không commit vào code
3. **Test trước:** Test với số tiền nhỏ trước
4. **Logs:** Kiểm tra logs để debug

---

**🎉 Bạn có thể deploy ngay với tài khoản merchant VNPay!**
