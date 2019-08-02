using System;
using System.Drawing;
using Rhino;
using Rhino.Commands;
using Rhino.UI;

namespace RhinoUI.Commands
{
    public class UiShowLayerPanel : Command
    {
        static UiShowLayerPanel _instance;
        public UiShowLayerPanel()
        {
            // register LayerPanel
            Panels.RegisterPanel(PlugIn, typeof(UI.Views.LayerPanel), "Layer panel", new Icon("D:\\Git\\RhinoUI\\RhinoUI\\EmbeddedResources\\plugin-utility.ico"));
            _instance = this;
        }

        ///<summary>The only instance of the UiShowLayerPanel command.</summary>
        public static UiShowLayerPanel Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "UiShowLayerPanel"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // CHeck visible
            var panelId = UI.Views.LayerPanel.PanelID;
            var visible = Panels.IsPanelVisible(panelId);

            var prompt = (visible)
                ? "Layer Panel is visible"
                : "Layer Panel is hidden";

            RhinoApp.WriteLine(prompt);

            // toggle visible
            if (!visible) Panels.OpenPanel(panelId);
            else Panels.ClosePanel(panelId);

            return Result.Success;
        }
    }
}