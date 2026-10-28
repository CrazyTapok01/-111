using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioMale.Checked = true;   // пол всегда определён
            txtAge.Enabled = false;     // возраст доступен только с галочкой
            chkAge.CheckedChanged += (s, ev) => txtAge.Enabled = chkAge.Checked;
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            // ---- 1. Ввод и проверка ----
            if (!double.TryParse(textWeight.Text.Replace(',', '.'), NumberStyles.Float,
                CultureInfo.InvariantCulture, out double weight) || weight <= 0)
            {
                MessageBox.Show("Введите корректный вес!");
                return;
            }

            if (!double.TryParse(textHeight.Text.Replace(',', '.'), NumberStyles.Float,
                CultureInfo.InvariantCulture, out double heightCm) || heightCm <= 0)
            {
                MessageBox.Show("Введите корректный рост!");
                return;
            }

            if (!radioMale.Checked && !radioFemale.Checked)
            {
                MessageBox.Show("Выберите пол!");
                return;
            }

            bool isMale = radioMale.Checked;
            string genderText = isMale ? "мужской" : "женский";

            // ---- 2. Расчёт ИМТ ----
            double bmi = weight / Math.Pow(heightCm / 100.0, 2);

            // ---- 3. Границы нормы ----
            double normMin = 0;
            double normMax = 0;
            string source;

            if (chkAge.Checked)
            {
                if (!int.TryParse(txtAge.Text, out int age) || age < 19 || age > 120)
                {
                    MessageBox.Show("Введите корректный возраст! (19-120)");
                    txtAge.Focus();
                    return;
                }

                // таблица возраста: 19-24, 25-34, 35-44, 45-54, 55-64, 65+
                int[] ageMax = { 24, 34, 44, 54, 64, 120 };
                double[] ageNormMin = { 19, 20, 21, 22, 23, 24 };
                double[] ageNormMax = { 24, 25, 26, 27, 28, 29 };

                for (int i = 0; i < ageMax.Length; i++)
                {
                    if (age <= ageMax[i])
                    {
                        normMin = ageNormMin[i];
                        normMax = ageNormMax[i];
                        break;
                    }
                }

                source = $"возраст {age}";
            }
            else
            {
                // таблица по полу
                normMin = isMale ? 20 : 19;
                normMax = isMale ? 25 : 24;
                source = "по полу";
            }

            // ---- 4. Категория ----
            string category;
            Color ctgColor;

            if (bmi < normMin)
            {
                category = "Недостаток";
                ctgColor = Color.DodgerBlue;
            }
            else if (bmi <= normMax)
            {
                category = "Норма";
                ctgColor = Color.LimeGreen;
            }
            else if (bmi <= 30)
            {
                category = "Избыток";
                ctgColor = Color.Orange;
            }
            else
            {
                category = "Ожирение";
                ctgColor = Color.Red;
            }

            // ---- 5. Классификация ВОЗ (цикл по таблице) ----
            double[] whoLimits = { 16, 18.5, 25, 30, 35, 40 };
            string[] whoNames =
            {
                "Выраженный дефицит массы",
                "Недостаточная масса",
                "Норма",
                "Избыточная масса",
                "Ожирение I степени",
                "Ожирение II степени",
                "Ожирение III степени"
            };

            string who = whoNames[whoNames.Length - 1];
            for (int i = 0; i < whoLimits.Length; i++)
            {
                if (bmi < whoLimits[i])
                {
                    who = whoNames[i];
                    break;
                }
            }

            // ---- 6. Вывод результатов ----
            lblResult.Text = $"ИМТ = {bmi:F2}\n" +
                             $"Пол: {genderText}\n" +
                             $"Норма ({source}): {normMin}-{normMax}\n" +
                             $"ВОЗ: {who}";

            lblCategory.Text = category;
            lblCategory.BackColor = ctgColor;
            lblCategory.ForeColor = Color.White;

            pictureBox2.BackColor = ctgColor;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }
    }
}
