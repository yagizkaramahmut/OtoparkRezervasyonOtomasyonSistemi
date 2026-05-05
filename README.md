# 🚗 Otopark Rezervasyon Otomasyon Sistemi

Bu proje, C# Windows Forms ve SQL Server kullanılarak geliştirilmiş, modern yazılım mühendisliği prensiplerini (Sorumlulukların Ayrılması / MVC yaklaşımı) barındıran profesyonel bir otopark yönetim ve rezervasyon sistemidir.

## 🌟 Öne Çıkan Özellikler

* **Katmanlı Mimari (N-Tier & MVC Yaklaşımı):** Veri tabanı bağlantıları (`DbConnection`) ve oturum yönetimleri (`OturumBilgi`) arayüzden tamamen izole edilerek temiz kod (Clean Code) standartlarına uyulmuştur.
* **Dinamik Kroki ve Çalışma Zamanı (Runtime) Üretim:** Otopark alanları (Butonlar) sürükle-bırak yöntemiyle değil, SQL'den gelen verilere göre çalışma zamanında dinamik olarak üretilir.
* **Akıllı Zaman ve Kademeli Fiyat Algoritması:** Müşterinin seçtiği tarih aralığı hesaplanarak; aylık, haftalık, günlük ve saatlik dilimlere parçalanır ve ticari olarak en doğru fiyat anlık olarak ekrana yansıtılır.
* **Google SMTP Mail Entegrasyonu:** Gerçekleştirilen rezervasyon işlemleri, müşterinin e-posta adresine resmi bir bilgilendirme metni olarak anında iletilir.
* **İlişkisel Veri Tabanı ve Korumalı Silme (Cascade):** `INNER JOIN` yapılarıyla tablolar arası ilişkiler kurulmuş olup, silme işlemlerinde referans bütünlüğünü (Foreign Key Constraints) korumak adına gelişmiş zırhlı algoritmalar yazılmıştır.
* **İndirim Kuponu Modülü:** Yöneticinin belirlediği oransal (Float) kuponlar, müşteriler tarafından kullanılarak fiyata anlık indirim olarak yansıtılır.

## 🛠️ Kullanılan Teknolojiler
* **Dil:** C# (.NET Framework)
* **Veri Tabanı:** Microsoft SQL Server (Relational DB)
* **Arayüz:** Windows Forms (TabControl Mimarisi)
* **Ağ Protokolleri:** System.Net.Mail (SMTP)

## 👨‍💻 Kurulum ve Çalıştırma
1. Projeyi klonlayın.
2. `DataAccess/DbConnection.cs` dosyası içerisindeki `connectionString` alanını kendi SQL Server yapılandırmanıza göre güncelleyin.
3. Proje klasöründeki SQL Script dosyasını çalıştırarak tabloları oluşturun.
4. Visual Studio üzerinden projeyi derleyip (Build) çalıştırın.
