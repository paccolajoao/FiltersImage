using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ImageFilters
{
    

    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }   

        private void button1_Click(object sender, EventArgs e)
        {
            
            Bitmap pic = Filter.CarregaImagem();

            panel1.BackgroundImage = pic;
            VerificaEvento(sender, e, pic);

        }

        

        private void VerificaEvento (object sender, EventArgs e, Bitmap pic)
        {
            if (panel1.BackgroundImage != null)
            {
                if (radioButton1.Checked == true)
                {
                    panel2.BackgroundImage = Filter.PretoBranco(pic);
                    
                }

                else if(radioButton2.Checked == true)

                {
                    panel2.BackgroundImage = Filter.SepiaTone(pic);
                    
                }

                else if (radioButton3.Checked == true)

                {
                    panel2.BackgroundImage = Filter.Invertido(pic);

                }

                else if (radioButton4.Checked == true)
                {
                    panel2.BackgroundImage = Filter.Contraste(pic,200);
                }

                else if (radioButton5.Checked == true)
                {
                    panel2.BackgroundImage = Filter.Contraste(pic, -40);
                }

                else if (radioButton6.Checked == true)
                {
                    panel2.BackgroundImage = Filter.FuzzyBlur(pic);
                }

                else if (radioButton7.Checked == true)
                {
                    panel2.BackgroundImage = Filter.Cartoon(pic);
                }


            }

            
        }

        private void Button2_Click(object sender, EventArgs e)
        {
           
            Bitmap pic = (Bitmap)panel1.BackgroundImage;
            VerificaEvento(sender, e, pic);
        }

       
    }
}
