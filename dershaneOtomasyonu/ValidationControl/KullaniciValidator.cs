using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using dershaneOtomasyonu.Database.Tables;

public class KullaniciValidator : AbstractValidator<Kullanici>
{
    public KullaniciValidator()
    {
        RuleFor(k => k.KullaniciAdi)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
            .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.");

        RuleFor(k => k.Adi)
            .NotEmpty().WithMessage("Adı boş olamaz.")
            .MaximumLength(50).WithMessage("Adı 50 karakterden uzun olamaz.");

        RuleFor(k => k.Soyadi)
            .NotEmpty().WithMessage("Soyadı boş olamaz.")
            .MaximumLength(50).WithMessage("Soyadı 50 karakterden uzun olamaz.");

        RuleFor(k => k.Tcno)
            .NotEmpty().WithMessage("T.C. Kimlik numarası boş olamaz.")
            .Length(11).WithMessage("T.C. Kimlik numarası 11 karakter uzunluğunda olmalıdır.")
            .Matches(@"^\d+$").WithMessage("T.C. Kimlik numarası sadece rakamlardan oluşmalıdır.");

        RuleFor(k => k.Email)
            .NotEmpty().WithMessage("Email boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

        RuleFor(k => k.Telefon)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Length(9).WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(k => k.DogumTarihi)
            .NotEmpty().WithMessage("Doğum tarihi boş olamaz.")
            .LessThan(DateTime.Now).WithMessage("Doğum tarihi bugünden küçük olmalıdır.");

        RuleFor(k => k.Adres)
            .MaximumLength(200).WithMessage("Adres 200 karakterden uzun olamaz.");
    }
}
