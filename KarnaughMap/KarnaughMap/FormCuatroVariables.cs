using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KarnaughMap
{
    public partial class FormCuatroVariables : Form
    {

        private readonly int nroVariables;


        HashSet<long> oNSet = new HashSet<long>();
        Dictionary<string, long> Valores = new Dictionary<string, long>();

        public FormCuatroVariables()
        {
            InitializeComponent();
        }

        public FormCuatroVariables(int nroVariables)
        {
            InitializeComponent();
            this.nroVariables = nroVariables;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ((Button)sender).Text = ChangeBinaryValue(((Button)sender).Text);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            GetBinaryValuesTrue();
            new FormMapaFuncion(nroVariables, oNSet, Valores).ShowDialog();
        }

        private void FormCuatroVariables_Load(object sender, EventArgs e)
        {

        }

        private HashSet<long> GetBinaryValuesTrue()
        {
            oNSet.Clear();
            Valores.Clear();

            var buttonCollection = GetAll(this, typeof(Button));

            foreach (var item in buttonCollection)
            {
                if (item.Tag != null)
                {
                    if ((int.Parse(item.Tag.ToString()) >= 0) && (int.Parse(item.Tag.ToString()) <= 15))
                    {
                        if (item.Text == "1")
                        {
                            oNSet.Add(long.Parse(item.Tag.ToString()));
                            Valores.Add(item.Name.ToString(), long.Parse(item.Tag.ToString()));
                        }
                    }
                }
            }

            return oNSet;
        }


        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        private string ChangeBinaryValue(string valor)
        {
            return valor == "0" ? "1" : "0";
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
