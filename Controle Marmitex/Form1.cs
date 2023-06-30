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
using System.Collections;

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
        string refriselec;
        public Form1()
        {
            InitializeComponent();
            InitializeProducts();
            numericUpDown1.Value = 1;
            textBox1.Visible = false;
            checkBox1.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
        }

        public void InitializeProducts(){
            products = new Dictionary<string, decimal>
            {
                { "Marmitex P", 13.0m },
                { "Marmitex P c/ churrasco", 15.0m },
                { "Marmitex G", 17.0m },
                { "Marmitex G - 14,00", 14.0m },
                { "Marmitex G c/ churrasco", 20.0m },
                { "Porção de churrasco", 9.0m },
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
            if (selectedProduct == "Marmitex G - 14,00")
            {
                string result = string.Join(",", checkboxTexts);
                decimal price = products[selectedProduct];
                total = quantidade * price;
                string cartItem = $"{quantidade}x - Marmitex G ({result}) = {total:C2}";
                totalprice += total;
                listBox1.Items.Add(cartItem);
                numericUpDown1.Value = 1;
                desmarcarCheckbox();
            } else if (selectedProduct == "Refrigerante mini" || selectedProduct == "Refrigerante Lata" || 
                selectedProduct == "Refrigerante 600ml" || selectedProduct == "Refrigerante 1L" || selectedProduct == "Refrigerante 2L")
            {
                refriselec = textBox1.Text;
                decimal price = products[selectedProduct];
                total = quantidade * price;
                string cartItem = $"{quantidade}x - {selectedProduct} ({refriselec}) = {total:C2}";
                totalprice += total;
                listBox1.Items.Add(cartItem);
                numericUpDown1.Value = 1;
            } else if (selectedProduct == "Porção de churrasco"){
                refriselec = textBox1.Text;
                decimal gramas = Decimal.Parse(refriselec);
                decimal price = products[selectedProduct];
                total = gramas * (price/100);
                string cartItem = $"{quantidade}x - {selectedProduct} ({refriselec} gramas) = {total:C2}";
                totalprice += total;
                listBox1.Items.Add(cartItem);
                numericUpDown1.Value = 1;
            }
                else {
                string result = string.Join(", ", checkboxTexts);
                decimal price = products[selectedProduct];
                total = quantidade * price;
                string cartItem = $"{quantidade}x - {selectedProduct} ({result}) = {total:C2}";
                totalprice += total;
                listBox1.Items.Add(cartItem);
                numericUpDown1.Value = 1;
                desmarcarCheckbox();
            }
        }

        public decimal GetTotalFromCartItem(string cartItem)
        {
            string[] parts = cartItem.Split('=');
            if (parts.Length == 2 && decimal.TryParse(parts[1].Trim().Replace("R$", ""), out decimal total))
            {
                return total;
            }
            return 0.0m;
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
            if (marmitex == "Refrigerante mini" || marmitex == "Refrigerante Lata" ||
                marmitex == "Refrigerante 600ml" || marmitex == "Refrigerante 1L" || marmitex == "Refrigerante 2L")
            {
                label9.Visible = true;
                textBox1.Visible = true;
                checkBox1.Visible = false;
                label8.Visible = false;
            } else if (marmitex == "Marmitex P" || marmitex == "Marmitex P c/ churrasco" || marmitex == "Marmitex G" || marmitex == "Marmitex G - 14,00" || marmitex == "Marmitex G c/ churrasco") 
            {
                label9.Visible = false;
                textBox1.Visible = false;
                checkBox1.Visible = true;
                label8.Visible = false;
            } else if (marmitex == "Porção de churrasco")
            {
                label8.Visible= true;
                label9.Visible = false;
                textBox1.Visible = true;
                checkBox1.Visible = false;
            }
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
            ImpressoraNaoFiscal impressoraNaoFiscal = new ImpressoraNaoFiscal(ModeloImpressoraNaoFiscal.MP4000TH, "COM4");
            TextoFormatado data = new TextoFormatado($"Data: {DateTime.Now} \n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido, TextoFormatado.TipoAlinhamento.Centralizado);
            TextoFormatado cliente = new TextoFormatado($"Cliente: {nome} \n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido);
            impressoraNaoFiscal.Imprimir(data);
            impressoraNaoFiscal.Imprimir("\n");
            impressoraNaoFiscal.Imprimir(cliente);
            impressoraNaoFiscal.Imprimir("\n");
            impressoraNaoFiscal.Imprimir("Itens vendidos: \n");
            foreach (object item in listBox1.Items)
            {
                TextoFormatado items = new TextoFormatado(item.ToString() + "\n \n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido);
                impressoraNaoFiscal.Imprimir(items);
            }
            TextoFormatado enderes = new TextoFormatado($"Endereço: {endereço1}\n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido);
            impressoraNaoFiscal.Imprimir("\n");
            impressoraNaoFiscal.Imprimir(enderes);
            impressoraNaoFiscal.Imprimir("\n");
            TextoFormatado pag2 = new TextoFormatado($"Pagamento: {pag}\n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido);
            impressoraNaoFiscal.Imprimir(pag2);
            impressoraNaoFiscal.Imprimir("\n");
            TextoFormatado value = new TextoFormatado($"Valor final: {totalprice:C2}\n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido);
            impressoraNaoFiscal.Imprimir(value);
            impressoraNaoFiscal.Imprimir("\n");
            TextoFormatado observs = new TextoFormatado($"Observações: {observaçoes:C2}\n", TextoFormatado.TamanhoCaracter.Normal, TextoFormatado.FormatoCaracter.Expandido);
            impressoraNaoFiscal.Imprimir(observs);
            impressoraNaoFiscal.CortarPapel(false);
        }

        private void button2_Click(object sender, EventArgs e){
            Imprimir();
            textnome.Text = "";
            endereço.Text = "";
            observ.Text = "";
            totalprice = 0;
            listBox1.Items.Clear();
        }

        private void removerButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex >= 0)
            {
                string selectedItem = listBox1.Items[selectedIndex].ToString();
                decimal totalToRemove = GetTotalFromCartItem(selectedItem);
                listBox1.Items.RemoveAt(selectedIndex);
                totalprice -= totalToRemove;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            refriselec = textBox1.Text;
        }

        private void feijoada_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tutu_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void macarrao_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void macarronada_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void batatatempassada_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
