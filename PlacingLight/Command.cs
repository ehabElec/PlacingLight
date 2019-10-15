#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion

namespace PlacingLight
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Revit application documents. 
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;
            
            try
            {
                Element el = SelectElement(uidoc, doc);
                FamilyInstance lightFamilyInstance = el as FamilyInstance;

                FamilySymbol lightFamilySymbol = lightFamilyInstance.Symbol;
                FamilyPlacementType placementType = lightFamilySymbol.Family.FamilyPlacementType;


                Reference hostFace = lightFamilyInstance.HostFace;



                //XYZ Bo1 = uidoc.Selection.PickPoint();
                XYZ Po1 = uidoc.Selection.PickPoint("Select point to place new light:");





                XYZ Bo2 = uidoc.Selection.PickPoint();
                LocationPoint lp = lightFamilyInstance.Location as LocationPoint;
                Po1 = new XYZ(Po1.X, Po1.Y, lp.Point.Z);
                Bo2 = new XYZ(Bo2.X, Bo2.Y, lp.Point.Z);


                // Modify document within a transaction

                int N;
                bool retN = CollectDataInput("Please input an integer:", out N);
                int M;
                bool retM = CollectDataInput("Please input an integer:", out M);


                using (Transaction tx = new Transaction(doc))
                {
                   // int N = 2, M = 2;
                    double z, x1, x2, xt, x, xp, xs, y1, y2, yt, y, yp;
                    z = Po1.Z;

                    x1 = Po1.X; y1 = Po1.Y;
                    x2 = Bo2.X; y2 = Bo2.Y;
                    xt = x2 - x1;
                    yt = y2 - y1;
                    y = yt / (2 * N);
                    x = xt / (2 * M);
                    xp = x1 + x;
                    xs = xp;
                    yp = y1 + y;


                    int i = 1, j = 1;
                    for (i = 1; i <= N; i++)
                    {

                        xp = xs;
                        for (j = 1; j <= M; j++)
                        {

                            XYZ pickPoint = new XYZ(xp, yp, z);

                            tx.Start("Placing Light");
                            FamilyInstance lightFamilyInstance2 = doc.Create.
                                NewFamilyInstance(hostFace, pickPoint, XYZ.BasisX, lightFamilySymbol);
                            tx.Commit();

                            xp = xp + 2 * x;

                        }
                        yp = yp + 2 * y;

                    }

                }

            }
            catch (OperationCanceledException) { }



            return Result.Succeeded;
        }

        public Element SelectElement(UIDocument uidoc, Document doc)
        {


            Reference Ref1 = uidoc.Selection.PickObject(ObjectType.Element);
            Element el = doc.GetElement(Ref1);
            return el;


        }


        public static bool CollectDataInput(string title, out int ret)
        {
            System.Windows.Forms.Form dc = new System.Windows.Forms.Form();
            dc.Text = title;

            dc.HelpButton = dc.MinimizeBox = dc.MaximizeBox = false;
            dc.ShowIcon = dc.ShowInTaskbar = false;
            dc.TopMost = true;

            dc.Height = 100;
            dc.Width = 300;
            dc.MinimumSize = new Size(dc.Width, dc.Height);

            int margin = 5;
            Size size = dc.ClientSize;

            System.Windows.Forms.TextBox tb = new System.Windows.Forms.TextBox();
            tb.TextAlign = HorizontalAlignment.Right;
            tb.Height = 20;
            tb.Width = size.Width - 2 * margin;
            tb.Location = new System.Drawing.Point(margin, margin);
            tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dc.Controls.Add(tb);

            Button ok = new Button();
            ok.Text = "Ok";
            ok.Click += new EventHandler(ok_Click);
            ok.Height = 23;
            ok.Width = 75;
            ok.Location = new System.Drawing.Point(size.Width / 2 - ok.Width / 2, size.Height / 2);
            ok.Anchor = AnchorStyles.Bottom;
            dc.Controls.Add(ok);
            dc.AcceptButton = ok;

            dc.ShowDialog();

            return int.TryParse(tb.Text, out ret);
        }

        private static void ok_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Form form = (sender as System.Windows.Forms.Control).Parent as System.Windows.Forms.Form;
            form.DialogResult = DialogResult.OK;
            form.Close();
        }




    }
}

