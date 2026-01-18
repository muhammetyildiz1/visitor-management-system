using form.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class ApiSonuc
        {
            public string SonucKodu { get; set; }
            public string SonucMesaji { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string ZiyaretEdilenKisi { get; set; }
            public string ZiyaretSebebi { get; set; }
            public string GirisTarihi { get; set; }
        }

        public class ZiyaretciVerisi
        {
            public string TcKimlikNo { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string ZiyaretEdilenKisi { get; set; }
            public string ZiyaretSebebi { get; set; }
            public DateTime GirisTarihi { get; set; }
            public DateTime? CikisTarihi { get; set; }
            public string Durum { get; set; }
        }

        public class ListeApiSonuc
        {
            public string SonucKodu { get; set; }
            public List<ZiyaretciVerisi> Liste { get; set; }
        }

        public class ApiCevap
        {
            public Sonuc sonuc { get; set; }
            public Ziyaretci data { get; set; }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(saatVeTarih);
            thread.IsBackground = true;
            thread.Start();

            PanelleriGuncelle();

            try
            {
                RegistryKey rKey = Registry.CurrentUser.OpenSubKey(@"Software\ZiyaretciTakip\Ayarlar");
                if (rKey != null)
                {
                    object gelenVeri = rKey.GetValue("SonGirilenTC");
                    if (gelenVeri != null)
                    {
                        labelSonTC.Text = gelenVeri.ToString();
                        labelSonTC.Visible = true;
                        labelSonTC_Cikis.Text = gelenVeri.ToString();
                        labelSonTC_Cikis.Visible = true;
                    }
                    rKey.Close();
                }
            }
            catch { }
        }
        private void labelSonTC_Click(object sender, EventArgs e)
        {
            textBox1.Text =labelSonTC.Text;
            textBox1.Focus();
        }

        private void labelSonTC_Cikis_Click(object sender, EventArgs e)
        {
            textBox6.Text = labelSonTC_Cikis.Text;
            textBox6.Focus();
        }
        private async void PanelleriGuncelle()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var cevap = await client.GetAsync("https://localhost:7106/api/Ziyaretci/ZiyaretciListele");
                    string gelenString = await cevap.Content.ReadAsStringAsync();
                    var secenekler = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var sonuc = JsonSerializer.Deserialize<ListeApiSonuc>(gelenString, secenekler);

                    if (sonuc != null && sonuc.Liste != null)
                    {
                        int iceride = sonuc.Liste.Count(x => x.Durum == "İçeride");
                        int disarida = sonuc.Liste.Count(x => x.Durum == "Çıkış Yaptı");
                        int toplam = iceride + disarida;

                        labelIceride.Text = iceride.ToString();
                        labelDisarida.Text = disarida.ToString();
                        labelToplam.Text = toplam.ToString();
                        ListeGuncelle();
                    }
                }
            }
            catch { }
        }
        private async void ListeGuncelle()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var cevap = await client.GetAsync("https://localhost:7106/api/Ziyaretci/ZiyaretciListele");

                    if (cevap.IsSuccessStatusCode)
                    {
                        string gelenString = await cevap.Content.ReadAsStringAsync();
                        var secenekler = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var sonuc = JsonSerializer.Deserialize<ListeApiSonuc>(gelenString, secenekler);

                        if (sonuc != null && sonuc.Liste != null)
                        {
                            listeTablosu.DataSource = null;
                            listeTablosu.DataSource = sonuc.Liste;

                            foreach (DataGridViewRow satir in listeTablosu.Rows)
                            {
                                if (satir.Cells["GirisTarihi"].Value != null)
                                {
                                    DateTime gTarih = Convert.ToDateTime(satir.Cells["GirisTarihi"].Value);
                                    satir.Cells["GirisTarihi"].Value = gTarih.ToString("dd.MM.yyyy HH:mm");
                                }

                                if (satir.Cells["CikisTarihi"].Value != null && !string.IsNullOrEmpty(satir.Cells["CikisTarihi"].Value.ToString()))
                                {
                                    DateTime cTarih = Convert.ToDateTime(satir.Cells["CikisTarihi"].Value);
                                    satir.Cells["CikisTarihi"].Value = cTarih.ToString("dd.MM.yyyy HH:mm");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
        void saatVeTarih()
        {
            while (true)
            {
                try
                {
                    labelSaat.Invoke(new Action(() => labelSaat.Text = DateTime.Now.ToString("HH:mm:ss")));
                    labelTarih.Invoke(new Action(() => labelTarih.Text = DateTime.Now.ToString("dd.MM.yyyy")));
                }
                catch
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            long sayi;
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.Length != 11 || !long.TryParse(textBox1.Text, out sayi))
            {
                MessageBox.Show("Lütfen 11 haneli ve sadece rakamlardan oluşan TC giriniz.");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Lütfen adı giriniz.");
                return;
            }
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Lütfen soyadı giriniz.");
                return;
            }
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Lütfen ziyaret edilen kişiyi giriniz.");
                return;
            }
            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Lütfen ziyaret sebebini giriniz.");
                return;
            }

            try
            {
                var gidenVeri = new
                {
                    TcKimlikNo = textBox1.Text,
                    Ad = textBox2.Text,
                    Soyad = textBox3.Text,
                    ZiyaretEdilenKisi = textBox4.Text,
                    ZiyaretSebebi = textBox5.Text,
                    GirisTarihi = DateTime.Now,
                    Durum = "İçeride"
                };

                string json = JsonSerializer.Serialize(gidenVeri);
                var icerik = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    var cevap = await client.PostAsync("https://localhost:7106/api/Ziyaretci/ZiyaretciGiris", icerik);
                    string gelenString = await cevap.Content.ReadAsStringAsync();
                    var secenekler = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var apiCevap = JsonSerializer.Deserialize<ApiSonuc>(gelenString, secenekler);

                    if (cevap.IsSuccessStatusCode && apiCevap != null && apiCevap.SonucKodu == "001")
                    {
                        MessageBox.Show(apiCevap.SonucMesaji);
                        labelSonTC.Text = textBox1.Text;
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        textBox1.Focus();
                        PanelleriGuncelle();

                        string log = $"{gidenVeri.TcKimlikNo}    {gidenVeri.Ad}    {gidenVeri.Soyad}    Giriş:    {DateTime.Now:dd.MM.yyyy HH:mm}";
                        try
                        {
                            Process.Start("logTutucu.exe", $"\"{log}\"");
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show(apiCevap != null ? apiCevap.SonucMesaji : "Ziyaretçi girişi başarısız oldu.");
                    }
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata oluştu: " + hata.Message);
            }
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            string TcKimlikNo = textBox6.Text;
            long sayi;
            if (string.IsNullOrEmpty(TcKimlikNo) || TcKimlikNo.Length != 11 || !long.TryParse(TcKimlikNo, out sayi))
            {
                MessageBox.Show("Lütfen 11 haneli ve sadece rakamlardan oluşan TC giriniz.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string baglantiAdresi = "https://localhost:7106/api/Ziyaretci/ZiyaretciCikis?TcKimlikNo=" + TcKimlikNo;
                    var cevap = await client.DeleteAsync(baglantiAdresi);
                    string gelenString = await cevap.Content.ReadAsStringAsync();
                    var secenekler = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var apiCevap = JsonSerializer.Deserialize<ApiSonuc>(gelenString, secenekler);

                    if (cevap.IsSuccessStatusCode && apiCevap != null && apiCevap.SonucKodu == "003")
                    {
                        MessageBox.Show(apiCevap.SonucMesaji);
                        labelSonTC_Cikis.Text = TcKimlikNo;
                        textBox6.Clear();
                        textBox6.Focus();
                        PanelleriGuncelle();
                            
                        string log = $"{TcKimlikNo}    {apiCevap.Ad}    {apiCevap.Soyad}    Çıkış:    {DateTime.Now:dd.MM.yyyy HH:mm}";
                        try
                        {
                            Process.Start("logTutucu.exe", $"\"{log}\"");
                        } 
                        catch 
                        { 

                        }
                    }
                    else
                    {
                        MessageBox.Show(apiCevap != null ? apiCevap.SonucMesaji : "Ziyaretçi çıkışı başarısız oldu.");
                    }
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata oluştu: " + hata.Message);
            }
        }

        private void butonYenile_Click(object sender, EventArgs e)
        {
            ListeGuncelle();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 11)
            {
                MessageBox.Show("Lütfen 11 haneli TC kimlik numarası giriniz.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://localhost:7106/api/Ziyaretci/ZiyaretciAra?tc=" + textBox1.Text;

                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    ApiCevap gelenVeri = JsonSerializer.Deserialize<ApiCevap>(responseBody, options);

                    if (gelenVeri != null && gelenVeri.sonuc.SonucKodu == "001" && gelenVeri.data != null)
                    {
                        textBox2.Text = gelenVeri.data.Ad;
                        textBox3.Text = gelenVeri.data.Soyad;
                        textBox4.Text = gelenVeri.data.ZiyaretEdilenKisi;
                        textBox5.Text = gelenVeri.data.ZiyaretSebebi;
                    }
                    else
                    {
                        MessageBox.Show(gelenVeri?.sonuc.SonucMesaji ?? "Kayıt bulunamadı.");
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox1.Focus();
        }
    }
}