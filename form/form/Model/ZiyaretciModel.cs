using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace form.Model
{
    public class Ziyaretci
    {
        public Guid Id { get; set; }
        public string TcKimlikNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string ZiyaretSebebi { get; set; }
        public string ZiyaretEdilenKisi { get; set; }
        public DateTime GirisTarihi { get; set; }
        public DateTime? CikisTarihi { get; set; }
        public string Durum { get; set; }
    }
    public class Sonuc
    {
        public string SonucKodu { get; set; }
        public string SonucMesaji { get; set; }
    }
    public class ListeSonuc : Sonuc
    {
        public List<Ziyaretci> Liste { get; set; }
    }
}