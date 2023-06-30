using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.IO.Ports;
using Bematech;
using Bematech.MiniImpressoras;
using Bematech.Texto;

namespace Controle_Marmitex
{
    public partial class Form1 : Form
    {


        string nomePorta;
        decimal total;
        string nome;
        string endereço1;
        string observaçoes;
        string pag;
        string opçoes;
        string marmitex;
        private Dictionary<string, decimal> products;
        decimal totalprice; 
        public Form1()
        {
            InitializeComponent();
            InitializeProducts();
            numericUpDown1.Value = 1;

        }

        public void InitializeProducts()
        {
            products = new Dictionary<string, decimal>
            {
                { "Marmitex P", 13.0m },
                { "Marmitex P c/ churrasco", 15.0m },
                { "Marmitex G", 17.0m },
                { "Marmitex G,", 14.0m },
                { "Marmitex G c/ churrasco", 20.0m },
                { "Porção de churrasco 100g", 9.0m },
                { "Porção de churrasco 50g", 4.5m },
                { "Refrigerante mini", 2.5m },
                { "Refrigerante Lata", 5.0m },
                { "Refrigerante 600ml", 6.0m },
                { "Refrigerante 1L", 8.0m },
                { "Refrigerante 2L", 12.0m }, 
                
            };
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textnome_TextChanged(object sender, EventArgs e)
        {
            nome = textnome.Text;
        }

        private void endereço_TextChanged(object sender, EventArgs e)
        {
            endereço1 = endereço.Text.ToString();
        }

        private void observ_TextChanged(object sender, EventArgs e)
        {
            observaçoes = observ.Text.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pag = comboBox2.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> checkboxTexts = new List<string>();

            var checkBoxes = Controls.OfType<System.Windows.Forms.CheckBox>().OrderBy(c => c.TabIndex);

            foreach (var checkBox in checkBoxes) { 

                if (checkBox.Checked) {
                    checkboxTexts.Add(checkBox.Text);
                }
            }
            decimal quantidade = numericUpDown1.Value;
            string selectedProduct = comboBox1.SelectedItem?.ToString();
            string result = string.Join(", ", checkboxTexts);
            decimal price = products[selectedProduct];
            total = quantidade * price;
            string cartItem = $"{quantidade}x - {selectedProduct} ({result}) = {total:C2}";
            totalprice += total;
            listBox1.Items.Add(cartItem);
            numericUpDown1.Value = 1;
            desmarcarCheckbox();


        }

        public bool desmarcarCheckbox()
        {
            foreach (Control control in Controls)
            {
                if (control is System.Windows.Forms.CheckBox checkBox && checkBox.Checked)
                {
                    checkBox.Checked = false;
                }
            }
            return true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            marmitex = comboBox1.Text.ToString();
        }

        private void SaveReport()
        {
            string reportsFolderPath = Path.Combine(Application.StartupPath, "Relatorios");

            if (!Directory.Exists(reportsFolderPath))
            {
                Directory.CreateDirectory(reportsFolderPath);
            }
            string reportFilePath = Path.Combine(reportsFolderPath, "relatorio.txt");


            try
            {
                using (StreamWriter writer = new StreamWriter(reportFilePath, true))
                {
                    // Escrever o conteúdo do relatório no arquivo
                    writer.WriteLine($"Data: {DateTime.Now}");
                    writer.WriteLine($"Cliente: {nome}");
                    writer.WriteLine("Itens vendidos:");

                    foreach (object item in listBox1.Items)
                    {
                        writer.WriteLine(item.ToString());
                    }

                    writer.WriteLine($"Endereço: {endereço1}");
                    writer.WriteLine($"Pagamento: {pag}");
                    writer.WriteLine($"Valor final: {totalprice:C2}");
                    writer.WriteLine("----------");

                    MessageBox.Show("Relatório salvo com sucesso!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar o relatório: {ex.Message}");
            }
     
        }

        private void Imprimir()
        {
            
            TextoFormatado textoFormatado = new TextoFormatado();

            textoFormatado.Tamanho = TextoFormatado.TamanhoCaracter.Elite;
            ImpressoraNaoFiscal impressoraNaoFiscal = new ImpressoraNaoFiscal(ModeloImpressoraNaoFiscal.MP4000TH, "COM6");
            impressoraNaoFiscal.Imprimir($"Data: {DateTime.Now} \n");
            impressoraNaoFiscal.Imprimir($"Cliente: {nome} \n");
            impressoraNaoFiscal.Imprimir("Itens vendidos: \n");
            foreach (object item in listBox1.Items)
            {
                impressoraNaoFiscal.Imprimir(item.ToString() + "\n");
            }
            impressoraNaoFiscal.Imprimir($"Endereço: {endereço1}\n");
            impressoraNaoFiscal.Imprimir($"Pagamento: {pag}\n");
            impressoraNaoFiscal.Imprimir($"Valor final: {totalprice:C2}\n");
            impressoraNaoFiscal.Imprimir($"Observações: {observaçoes:C2}\n");
            impressoraNaoFiscal.CortarPapel(false);



        }

        private void button2_Click(object sender, EventArgs e)
        {
            Imprimir();
            textnome.Text = "";
            endereço.Text = "";
            observ.Text = "";
            totalprice = 0;
            listBox1.Items.Clear();
        }
    }
}
