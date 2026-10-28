using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AviaBooking
{
    public partial class Form1 : Form
    {
        // путь к папке "авиа" на рабочем столе
        string papka = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\авиа\";

        public Form1()
        {
            InitializeComponent();
        }

        // ===== ЭТАП 1. Запуск программы =====
        private void Form1_Load(object sender, EventArgs e)
        {
            rbEconomy.Checked = true;
            rbAdult.Checked = true;
            txtPassengers.Text = "1";

            chkBags.Checked = false;
            chkMeal.Checked = false;
            chkInsurance.Checked = false;

            lblResult.Text = "";
            txtPassengers.MaxLength = 2;

            // стартовый логотип (эконом)
            picLogo.Image = Image.FromFile(papka + "pobeda.png");   // расширение
        }

        // ===== ЭТАП 2.1. Выбор класса перелёта =====
        private void rbClass_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked) return;

            // меняем логотип
            if (rbEconomy.Checked)
                picLogo.Image = Image.FromFile(papka + "pobeda.png");     // расширение
            else if (rbBusiness.Checked)
                picLogo.Image = Image.FromFile(papka + "aeroflot.png");   // расширение
            else if (rbFirst.Checked)
                picLogo.Image = Image.FromFile(papka + "emirates.png");   // расширение

            // Первый класс: багаж и питание входят в тариф
            if (rbFirst.Checked)
            {
                chkBags.Checked = true;
                chkMeal.Checked = true;
                chkBags.Enabled = false;
                chkMeal.Enabled = false;
            }
            else
            {
                if (!chkBags.Enabled)
                {
                    chkBags.Enabled = true;
                    chkMeal.Enabled = true;
                    chkBags.Checked = false;
                    chkMeal.Checked = false;
                }
            }
        }

        // ===== ЭТАП 3. Кнопка «Забронировать» =====
        private void btnBook_Click(object sender, EventArgs e)
        {
            // --- Шаг 3.1. Валидация количества пассажиров ---
            int n;
            if (!int.TryParse(txtPassengers.Text, out n))
            {
                MessageBox.Show("Введите количество пассажиров числом!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (n < 1 || n > 10)
            {
                MessageBox.Show("Количество пассажиров должно быть от 1 до 10!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Шаг 3.2. Проверка класса перелёта ---
            string className;
            int basePrice;

            if (rbEconomy.Checked)
            {
                className = "Эконом";
                basePrice = 5000;
            }
            else if (rbBusiness.Checked)
            {
                className = "Бизнес";
                basePrice = 15000;
            }
            else if (rbFirst.Checked)
            {
                className = "Первый";
                basePrice = 30000;
            }
            else
            {
                MessageBox.Show("Выберите класс перелёта!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Шаг 3.3. Суммирование услуг ---
            int extras = 0;
            List<string> services = new List<string>();

            if (chkBags.Checked)
            {
                services.Add("багаж");
                if (!rbFirst.Checked) extras += 2000;
            }
            if (chkMeal.Checked)
            {
                services.Add("питание");
                if (!rbFirst.Checked) extras += 1500;
            }
            if (chkInsurance.Checked)
            {
                services.Add("страховка");
                extras += 800;
            }

            // --- Шаг 3.4. Цена одного билета ---
            int ticketPrice = basePrice + extras;

            // --- Шаг 3.5. Определение скидки ---
            double k = 0;
            string discountName = "нет";

            if (rbStudent.Checked)
            {
                k = 0.15;
                discountName = "студент 15%";
            }
            else if (rbChild.Checked)
            {
                k = 0.30;
                discountName = "ребёнок 30%";

                if (n == 1)
                {
                    MessageBox.Show("Ребёнок не может лететь один!",
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // --- Шаг 3.6. Итоговые расчёты ---
            double subtotal = n * ticketPrice;
            double discountSum = subtotal * k;
            double total = subtotal - discountSum;
            total = Math.Round(total);

            // --- Шаг 3.7. Текст результата ---
            string servicesText = services.Count > 0 ? string.Join(", ", services) : "нет";

            string result = "Класс: " + className + "\n" +
                            "Пассажиров: " + n + "\n" +
                            "Услуги: " + servicesText + "\n" +
                            "Цена 1 билета: " + ticketPrice.ToString("N0") + " ₽\n" +
                            "Без скидки: " + subtotal.ToString("N0") + " ₽\n" +
                            "Скидка (" + discountName + "): -" + discountSum.ToString("N0") + " ₽\n" +
                            "---------------------------\n" +
                            "ИТОГО: " + total.ToString("N0") + " ₽";

            lblResult.Text = result;
        }
    }
}
