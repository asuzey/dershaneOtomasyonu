﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dershaneOtomasyonu.Database;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250108173739_notlar-olusturmatarihi-addcolumn")]
    partial class notlarolusturmatarihiaddcolumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Degerlendirme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Aciklama")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<int>("Puan")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("KullaniciId");

                    b.ToTable("Degerlendirmeler");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Ders", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Aciklama")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Adi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Dersler");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.DersKayit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Durum")
                        .HasColumnType("bit");

                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<string>("Mesajlar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Oda")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SinifId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KullaniciId");

                    b.HasIndex("SinifId");

                    b.ToTable("DersKayitlari");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Dosya", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Dosyalar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Gorusme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Durum")
                        .HasColumnType("bit");

                    b.Property<int>("KatilimciId")
                        .HasColumnType("int");

                    b.Property<string>("Mesajlar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Oda")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OlusturucuId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KatilimciId");

                    b.HasIndex("OlusturucuId");

                    b.ToTable("Gorusmeler");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Kullanici", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Adres")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DogumTarihi")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KullaniciAdi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Sifre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SinifId")
                        .HasColumnType("int");

                    b.Property<string>("Soyadi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tcno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("SinifId");

                    b.ToTable("Kullanicilar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.KullaniciDers", b =>
                {
                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<int>("DersId")
                        .HasColumnType("int");

                    b.HasKey("KullaniciId", "DersId");

                    b.HasIndex("DersId");

                    b.ToTable("KullaniciDersleri");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.KullaniciDosya", b =>
                {
                    b.Property<int>("DosyaId")
                        .HasColumnType("int");

                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.HasKey("DosyaId", "KullaniciId");

                    b.HasIndex("KullaniciId");

                    b.ToTable("KullaniciDosyalari");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.KullaniciNot", b =>
                {
                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<int>("NotId")
                        .HasColumnType("int");

                    b.HasKey("KullaniciId", "NotId");

                    b.HasIndex("NotId");

                    b.ToTable("KullaniciNotlari");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("KullaniciId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Not", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Baslik")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Icerik")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OlusturmaTarihi")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Notlar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("RolAdi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roller");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Sinif", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Kodu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Siniflar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Yoklama", b =>
                {
                    b.Property<int>("DersKayitId")
                        .HasColumnType("int");

                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.HasKey("DersKayitId", "KullaniciId");

                    b.HasIndex("KullaniciId");

                    b.ToTable("Yoklamalar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Degerlendirme", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Creator")
                        .WithMany("OgretmenDegerlendirmeleri")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany("OgrenciDegerlendirmeleri")
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.DersKayit", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany("DersKayitlari")
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Sinif", "Sinif")
                        .WithMany()
                        .HasForeignKey("SinifId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kullanici");

                    b.Navigation("Sinif");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Gorusme", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Katilimci")
                        .WithMany("GorusmelerKatilimci")
                        .HasForeignKey("KatilimciId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Olusturucu")
                        .WithMany("GorusmelerOlusturucu")
                        .HasForeignKey("OlusturucuId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Katilimci");

                    b.Navigation("Olusturucu");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Kullanici", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Role", "Role")
                        .WithMany("Kullanicilar")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Sinif", "Sinif")
                        .WithMany()
                        .HasForeignKey("SinifId");

                    b.Navigation("Role");

                    b.Navigation("Sinif");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.KullaniciDers", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Ders", "Ders")
                        .WithMany("KullaniciDersleri")
                        .HasForeignKey("DersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany()
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ders");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.KullaniciDosya", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Dosya", "Dosya")
                        .WithMany()
                        .HasForeignKey("DosyaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany("Dosyalar")
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dosya");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.KullaniciNot", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany()
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Not", "Not")
                        .WithMany("KullaniciNotlari")
                        .HasForeignKey("NotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kullanici");

                    b.Navigation("Not");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.LogEntry", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany()
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Yoklama", b =>
                {
                    b.HasOne("dershaneOtomasyonu.Database.Tables.DersKayit", "DersKayit")
                        .WithMany("Yoklamalar")
                        .HasForeignKey("DersKayitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dershaneOtomasyonu.Database.Tables.Kullanici", "Kullanici")
                        .WithMany("Yoklamalar")
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DersKayit");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Ders", b =>
                {
                    b.Navigation("KullaniciDersleri");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.DersKayit", b =>
                {
                    b.Navigation("Yoklamalar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Kullanici", b =>
                {
                    b.Navigation("DersKayitlari");

                    b.Navigation("Dosyalar");

                    b.Navigation("GorusmelerKatilimci");

                    b.Navigation("GorusmelerOlusturucu");

                    b.Navigation("OgrenciDegerlendirmeleri");

                    b.Navigation("OgretmenDegerlendirmeleri");

                    b.Navigation("Yoklamalar");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Not", b =>
                {
                    b.Navigation("KullaniciNotlari");
                });

            modelBuilder.Entity("dershaneOtomasyonu.Database.Tables.Role", b =>
                {
                    b.Navigation("Kullanicilar");
                });
#pragma warning restore 612, 618
        }
    }
}
