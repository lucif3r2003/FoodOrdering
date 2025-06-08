using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FoodWeb.Helpers
{
    public static class VnPayHelper
    {
        private static readonly string vnp_TmnCode = "RLZVAGP9";  // Mã merchant của bạn
        private static readonly string vnp_HashSecret = "VIWKSCZP1K8IACC3IZCA5ZUZW0DIZFDX";   // Secret key từ VNPAY
        private static readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // Url VNPAY sandbox hoặc production
        private static readonly string vnp_ReturnUrl = "https://yourdomain.com/payment/vnpay_return"; // URL callback phải đúng với VNPAY config

        public static string CreatePaymentUrl(decimal amount, string orderInfo, string orderId, string ipAddress)
        {
            var vnp_Params = new Dictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() }, // amount tính bằng "xu" (100 VND = 1 đơn vị)
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", orderId },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },  // hoặc "en"
                { "vnp_ReturnUrl", vnp_ReturnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            // Sắp xếp theo key
            var sortedParams = vnp_Params.OrderBy(x => x.Key);

            var hashData = new StringBuilder();
            var query = new StringBuilder();

            foreach (var param in sortedParams)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    // Xây dựng chuỗi dữ liệu để hash (signature)
                    hashData.Append(param.Key + "=" + param.Value + "&");
                    // Xây dựng query string
                    query.Append(HttpUtility.UrlEncode(param.Key) + "=" + HttpUtility.UrlEncode(param.Value) + "&");
                }
            }

            // Bỏ dấu & cuối chuỗi
            hashData.Length--;
            query.Length--;

            string signData = hashData.ToString();

            // Tạo chữ ký bằng HMAC SHA256 với secret key
            string vnp_SecureHash = ComputeHmacSha256(vnp_HashSecret, signData);

            // Tạo URL thanh toán đầy đủ
            string paymentUrl = $"{vnp_Url}?{query}&vnp_SecureHashType=SHA256&vnp_SecureHash={vnp_SecureHash}";

            return paymentUrl;
        }

        private static string ComputeHmacSha256(string key, string data)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmacsha256.ComputeHash(dataBytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
