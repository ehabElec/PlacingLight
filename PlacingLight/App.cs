#region Namespaces
using System;
using System.Reflection;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endregion

namespace PlacingLight
{
    class App : IExternalApplication
    {
        static readonly string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static readonly string assyPath = Path.Combine(dir, "PlacingLight.dll");
        public Result OnStartup(UIControlledApplication a)
        {
            
            try
            {
                AddRibbonPanel(a);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ribbon", ex.ToString());
            }

            return Result.Succeeded;
        }

        private void AddRibbonPanel(UIControlledApplication app)
        {
            RibbonPanel panel = app.CreateRibbonPanel("By Ehab Bkheit ");

            PushButtonData pbd_PL = new PushButtonData("Placing Light ", "Placing Light", assyPath, "PlacingLight.Command");
            PushButton pb_PLbutton = panel.AddItem(pbd_PL) as PushButton;


            // Reflection of path to image 
            var globePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "PLicon.png");
            Uri uriImage = new Uri(globePath);
            // Apply image to bitmap
            BitmapImage largeImage = new BitmapImage(uriImage);
            // Apply image to button 
            pb_PLbutton.LargeImage = largeImage;

            pb_PLbutton.ToolTip = "Distribute hosted light family on false ceiling";
            pb_PLbutton.LongDescription = 
             " Select hosted light fixture in false ceiling \npick two-point  to specify a rectangular Room on the screen \nEnter number of Row Light then enter number of Column Light\nstandard light distrubtion will be placed on ceiling x-2x-x ";

            //ContextualHe/lp contextHelp = new ContextualHelp(ContextualHelpType.ChmFile, dir + "/Resources/STFExporter Help.htm");
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.Url, "https://github.com/ehabElec");

            pb_PLbutton.SetContextualHelp(contextHelp);
        }

        private ImageSource iconImage(string sourcename)
        {
            try
            {
                Stream icon = Assembly.GetExecutingAssembly().GetManifestResourceStream(sourcename);
                if (icon != null)
                {
                    PngBitmapDecoder decoder = new PngBitmapDecoder(
                      icon,
                      BitmapCreateOptions.PreservePixelFormat,
                      BitmapCacheOption.Default);


                    ImageSource source = decoder.Frames[0];
                    return source;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
