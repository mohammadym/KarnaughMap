using Karno;
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
    public partial class FormMapaFuncion : Form
    {
        private readonly int numeroVariables;
        private readonly HashSet<long> oNSet;
        Dictionary<string, long> Valores = new Dictionary<string, long>();
        Tuple<string,string> result;

        public FormMapaFuncion()
        {
            InitializeComponent();
        }

        public FormMapaFuncion(int NumeroVariables, HashSet<long> ONSet, Dictionary<string, long> valores)
        {
            InitializeComponent();
            numeroVariables = NumeroVariables;
            oNSet = ONSet;
            this.Valores = valores;
        }

        private void FormMapaFuncion_Load(object sender, EventArgs e)
        {
            var map = new KMap(numeroVariables, oNSet, new HashSet<long>() { });
            result = map.PrintCoverages(true);
            Mapa.Text = result.Item1;
            InsertValuesMap();
            ActivePanelMap();
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.numeroVariables == 3 && oNSet.Count() == 8)
                lblFuncion.Text = "F = 1";
            else if (this.numeroVariables == 4 && oNSet.Count() == 16)
            {
                lblFuncion.Text = "F = 1";
            }
            else
            {
                lblFuncion.Text = "F = " + result.Item2;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActivePanelMap()
        {
            pn3Variables.Visible = numeroVariables == 3;
            pn4Variables.Visible = !pn3Variables.Visible;
        }

        private void InsertValuesMap()
        {
            if (numeroVariables == 3)
            {
                var buttonCollection = GetAll(this.pn3Variables, typeof(TextBox));
                foreach (var item in Valores)
                    {
                        foreach (var item1 in buttonCollection)
                        {
                            if (item.Key.ToString().Replace("f", "") == item1.Name.Replace("txt", ""))
                            {
                                item1.Text = "      " + item.Value.ToString() + "\n\r" + "1";
                            }
                        }

                    }
            }
            else
            {
                var buttonCollection4 = GetAll(this.pn4Variables, typeof(TextBox));
                foreach (var item in Valores)
                {
                    foreach (var item1 in buttonCollection4)
                    {
                        if (item.Key.ToString().Replace("f", "") == item1.Name.Replace("txt", ""))
                        {
                            if (item.Value.ToString().Trim().Length == 1)
                            {
                                item1.Text = "      " + item.Value.ToString() + "\n\r" + "1";
                            }
                            else
                            {
                                item1.Text = "    " + item.Value.ToString() + "\n\r" + "1";
                            }
                        }

                    }

                }
            }
        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
    }
}
