Merhabalar bu proje UÜ Görsel Programlama dersim için geliştirilmiş olup geliştirmesinde sayın mentörüm Muhammed Rıfat Türkmen'in emeği büyüktür.
Otomasyon online mesajlaşma ve dosya gönderme gibi özelliklere sahip olup bu konuda da bazı sınırlaramalara sahip olduğundan henüz tamamlanmamıştır.
Bu projenin serverlarına ait kodlar yayınlanmayacaktır.

Özetle;

C# ile Windows Form'da geliştirdiğim bu projenin beni en çok geliştiren proje olduğunu düşünüyorum. Sebebi ise
pek çok farklı teknolojileri kullanmış olmam. Projem birden fazla yetki (admin,öğretmen,öğrenci) özelliğine sahip ve
her kullanıcı için ayrı sayfa tasarımları ve özellikler bulunmakta. Projem için MSSQL kullanırken bunu ADO.NET
şeklinde değil de daha düzenli olduğundan ve code first anlayışını beğendiğimden Entitiy Framework ile geliştirmeye
karar verdim. Entity kullanırken crud işlemleri için Linq komutlarını kullanmayı öğrendim. Migration sistemini hala
karışık bulsam da projem için gerçekleştirebildim. Sorgularımda kullanıcılara gereksiz veya özel bilgileri sunmamak
için DTO kullandım. Uygulamamdaki şifre sistemini hiçbir kullanıcıya göstermeyip Gmail SMTP kullanarak projemin
içine yazdığım random şifre üretme metoduyla sadece ilgili kullanıcıya ilettim. Projemin içinde öğretmenler ve
öğrenciler gerçek zamanlı olarak iletişime geçebilsin diye WebSocket teknolojisini kullanmayı tercih ettim, server
kurdum ve projemde de bu durumları oda mantığını kullanarak özelleştirdim. Öğretmenlerin öğrencilerine dosya
göndermesi için 3. parti bir uygulama kullanmasına gerek kalmaması için aynı ağda çalışan TCP/IP yöntemiyle
dosya gönderimini sağladım.
