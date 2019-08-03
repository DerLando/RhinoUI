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

            // setup event handlers
            Grid.MouseDoubleClick += On_GridDoubleClicked;
            Grid.CellClick += On_CellClick;

            // define columns
            // Name
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<Layer, string>(l => l.Name)},
                HeaderText = "Name",
                Editable = true
            });

            // Current active layer
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell{ Binding = Binding.Delegate<Layer, bool?>(l => l == Rhino.RhinoDoc.ActiveDoc.Layers.CurrentLayer)},
                HeaderText = "Current",
            });

            // Visible
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<Layer, bool?>(l => l.IsVisible)},
                HeaderText = "On",
                Editable = true
            });

            // Locked
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<Layer, bool?>(l => l.IsLocked)},
                HeaderText = "Locked",
                Editable = true
            });

            // Color
            var bitmapSize = Settings.LayerPanelColorBitmapSize;
            Grid.Columns.Add(new GridColumn
            {
                DataCell = new ImageViewCell { Binding = Binding.Property<Layer, Image>(l => new Bitmap(bitmapSize, bitmapSize, PixelFormat.Format24bppRgb, from index in Enumerable.Repeat(0, bitmapSize * bitmapSize) select l.Color.ToEto()))},
                HeaderText = "Color",
                Editable = true
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

            RhinoDoc.LayerTableEvent += On_LayerTableEvent;
        }

        private void On_CellClick(object sender, GridCellMouseEventArgs e)
        {
            // test if color
            if (e.Column == 4)
            {
                var layer = e.Item as Layer;
                var color = layer.Color;
                Dialogs.ShowColorDialog(ref color);
                layer.Color = color;
            }

            // test if material
            if (e.Column == 5)
            {
                // Do nothing for now
            }

            // test if linetype
            if (e.Column == 6)
            {
                var layer = e.Item as Layer;
                var lineTypeIndex = layer.LinetypeIndex;
                Dialogs.ShowSelectLinetypeDialog(ref lineTypeIndex, false);
                layer.LinetypeIndex = lineTypeIndex;
            }

            // test if print color
            if (e.Column == 7)
            {
                var layer = e.Item as Layer;
                var color = layer.PlotColor;
                Dialogs.ShowColorDialog(ref color);
                layer.PlotColor = color;
            }
        }

        /// <summary>
        /// Handles double clicks on a grid row
        /// Default behaviour is to set the layer corresponding to the double clicked row
        /// as the new active layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_GridDoubleClicked(object sender, MouseEventArgs e)
        {
            var rowIndices = Grid.SelectedRows.ToArray();
            if (rowIndices.Length == 1)
            {
                var selectedLayer = Grid.DataStore.ToArray()[rowIndices[0]] as Layer;
                RhinoDoc.ActiveDoc.Layers.SetCurrentLayerIndex(selectedLayer.Index, true);
            }
        }

        private void UpdateGridDataStore(RhinoDoc doc)
        {
            Grid.DataStore = doc.Layers;
        }

        private void On_LayerTableEvent(object sender, LayerTableEventArgs e)
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
