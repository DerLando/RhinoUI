using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.Render.Fields;
using Rhino.UI;

namespace RhinoUI.UI.Views
{
    [Guid("756979E5-C3B8-4A6A-9E86-846564F00C27")]
    public class LayerPanel : Panel, IPanel
    {
        private readonly uint _documentSerialNumber;

        private GridView Grid;

        public static Guid PanelID => typeof(LayerPanel).GUID;

        public LayerPanel(uint documentSerialNumber)
        {
            // assign documentSerialNumber to field
            _documentSerialNumber = documentSerialNumber;

            // get active doc
            var doc = RhinoDoc.ActiveDoc;

            // get layers to grid
            Grid = new GridView();
            UpdateGridDataStore(doc);

            // define columns
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<Layer, string>(l => l.Name)},
                HeaderText = "Name"
            });

            Grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell{ Binding = Binding.Delegate<Layer, bool?>(l => l == Rhino.RhinoDoc.ActiveDoc.Layers.CurrentLayer)},
                HeaderText = "Current"
            });

            Grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<Layer, bool?>(l => l.IsVisible)},
                HeaderText = "On"
            });

            Grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<Layer, bool?>(l => l.IsLocked)},
                HeaderText = "Locked"
            });

            // Color
            var bitmapSize = Settings.LayerPanelColorBitmapSize;
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new ImageViewCell { Binding = Binding.Property<Layer, Image>(l => new Bitmap(bitmapSize, bitmapSize, PixelFormat.Format24bppRgb, from index in Enumerable.Repeat(0, bitmapSize * bitmapSize) select l.Color.ToEto()))},
                HeaderText = "Color"
            });

            // Material
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell {Binding = Binding.Property<Layer, string>(l => l.RenderMaterial.Name)},
                HeaderText = "Material"
            });

            // Linetype
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell {Binding = Binding.Property<Layer, string>(l => doc.Linetypes[l.LinetypeIndex].Name)},
                HeaderText = "LineType"
            });

            // PrintColor
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new ImageViewCell { Binding = Binding.Property<Layer, Image>(l => new Bitmap(bitmapSize, bitmapSize, PixelFormat.Format24bppRgb, from index in Enumerable.Repeat(0, bitmapSize * bitmapSize) select l.PlotColor.ToEto())) },
                HeaderText = "Print Color"
            });

            // PrintWidth
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<Layer, string>(l => l.PlotWeight.ToString(CultureInfo.InvariantCulture))},
                HeaderText = "Print Width"
            });

            Content = Grid;

            RhinoDoc.LayerTableEvent += OnLayerTableEvent;
        }

        private void UpdateGridDataStore(RhinoDoc doc)
        {
            Grid.DataStore = doc.Layers;
        }

        private void OnLayerTableEvent(object sender, LayerTableEventArgs e)
        {
            UpdateGridDataStore(e.Document);
        }

        #region IPanel methods
        public void PanelShown(uint documentSerialNumber, ShowPanelReason reason)
        {
            // Called when the panel tab is made visible, in Mac Rhino this will happen
            // for a document panel when a new document becomes active, the previous
            // documents panel will get hidden and the new current panel will get shown.
            Rhino.RhinoApp.WriteLine($"Panel shown for document {documentSerialNumber}, this serial number {_documentSerialNumber} should be the same");
        }

        public void PanelHidden(uint documentSerialNumber, ShowPanelReason reason)
        {
            // Called when the panel tab is hidden, in Mac Rhino this will happen
            // for a document panel when a new document becomes active, the previous
            // documents panel will get hidden and the new current panel will get shown.
            Rhino.RhinoApp.WriteLine($"Panel hidden for document {documentSerialNumber}, this serial number {_documentSerialNumber} should be the same");
        }

        public void PanelClosing(uint documentSerialNumber, bool onCloseDocument)
        {
            // Called when the document or panel container is closed/destroyed
            Rhino.RhinoApp.WriteLine($"Panel closing for document {documentSerialNumber}, this serial number {_documentSerialNumber} should be the same");
        }
        #endregion IPanel methods
    }
}
