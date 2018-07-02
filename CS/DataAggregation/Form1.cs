using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAggregation {
    public partial class Form1 : DevExpress.XtraEditors.XtraForm {
        public Form1() {
            InitializeComponent();
        }

        List<DataPoint> GenerateData(int pointCount) {
            List<DataPoint> result = new List<DataPoint>(pointCount);
            double value = 0;
            DateTime argument = DateTime.Now.AddHours(-pointCount);
            Random random = new Random();
            
            for (double i = 0; i < pointCount; i++) {
                result.Add(new DataPoint { Argument = argument.AddHours(i), Value = Math.Abs(value) });
                value += (random.NextDouble() * 10.0 - 5.0);
            }
            return result;
        }

        #region #CustomAggregateFunction
        private void Form1_Load(object sender, EventArgs e) {
            Series series = chartControl.Series["Random Data"];
            series.DataSource = GenerateData(100_000);
            series.ArgumentDataMember = "Argument";
            series.ValueDataMembers.AddRange("Value", "Value", "Value", "Value");

            XYDiagram diagram = chartControl.Diagram as XYDiagram;
            diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.Custom;
            diagram.AxisX.DateTimeScaleOptions.CustomAggregateFunction = new OhlcAggregateFunction();
        }

        class OhlcAggregateFunction : CustomAggregateFunction {
            public override double[] Calculate(GroupInfo groupInfo) {
                double open = groupInfo.Values1.First();
                double close = groupInfo.Values1.Last();
                double high = Double.MinValue;
                double low = Double.MaxValue;
                foreach (double value in groupInfo.Values1) {
                    if (high < value) high = value;
                    if (low > value) low = value;
                }

                return new double[] { high, low, open, close };
            }
        }
        #endregion #CustomAggregateFunction

        class DataPoint {
            public DateTime Argument { get; set; }
            public double Value { get; set; }

        }
    }
}
