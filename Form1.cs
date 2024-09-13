using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Wordle
{
    public partial class Form1 : Form
    {
        private List<string> words;
        private string targetWord;
        private int attemptCount;

        public Form1()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(255, 18, 18, 19);
            words = LoadWords();
            targetWord = SelectRandomWord(words);
            attemptCount = 0;
            DisplayTargetWord();
        }

        private List<string> LoadWords()
        {
            List<string> words = new List<string>();
            try
            {
                string[] lines = File.ReadAllLines("words.txt");
                words = lines.Where(word => word.Length == 5).Select(word => word.ToUpper()).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kelime dosyas� okunamad�: " + ex.Message);
            }
            return words;
        }

        private string SelectRandomWord(List<string> words)
        {
            Random random = new Random();
            int index = random.Next(words.Count);
            return words[index];
                //.ToUpper(); // B�y�k harf ile standartla�t�rma
        }

        private void DisplayTargetWord()
        {
            label1.Text = targetWord; // Rastgele se�ilen kelimeyi g�ster
        }

        private void NewGame()
        {
            targetWord = SelectRandomWord(words); // Yeni kelime se�
            attemptCount = 0; // Tahmin say�s�n� s�f�rla
            flpGuesses.Controls.Clear();  // Tahmin ge�mi�ini temizle
            DisplayTargetWord();
            lblAttempts.Text = "Tahmin Say�s�:";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string guess = txtGuess.Text.ToUpper().Trim();

            if (guess.Length != 5)
            {
                MessageBox.Show("Tahmin 5 harfli olmal�d�r.");
                return;
            }

            if (!words.Contains(guess))
            {
                MessageBox.Show("Tahmin ge�ersiz bir kelime.");
                return;
            }

            attemptCount++;
            DisplayGuessResult(guess, targetWord); //ekrana yazd�r�yor
            lblAttempts.Text = $"Tahmin Say�s�: {attemptCount}";

            if (guess.Equals(targetWord))
            {
                MessageBox.Show("Tebrikler! Do�ru tahmin ettiniz.");
                NewGame();
            }


            if (attemptCount == 6)
            {
                MessageBox.Show($"Bilemediniz. Do�ru kelime: {targetWord}");
                NewGame();
            }

            txtGuess.Focus();
            txtGuess.Text = "";
        }



        private void DisplayGuessResult(string guess, string targetWord)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false
            };

            for (int i = 0; i < guess.Length; i++)
            {
                Label letterLabel = new Label
                {
                    Text = guess[i].ToString(),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Width = 30,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                };

                if (guess[i] == targetWord[i])
                {
                    letterLabel.BackColor = Color.FromArgb(255, 83, 141, 78); // Do�ru harf ve konum
                    letterLabel.ForeColor = Color.White;
                }
                else if (targetWord.Contains(guess[i]))
                {
                    letterLabel.BackColor = Color.FromArgb(255, 181, 159, 59); // Do�ru harf ama yanl�� konum
                    letterLabel.ForeColor = Color.White;
                }
                else
                {
                    letterLabel.BackColor = Color.FromArgb(255, 58, 58, 60); // Yanl�� tahmin
                    letterLabel.ForeColor = Color.White;
                }

                panel.Controls.Add(letterLabel);
            }

            flpGuesses.Controls.Add(panel);
        }
        
        private void txtGuess_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSubmit.PerformClick();
                e.Handled = true;
            }
        }
    }
}
