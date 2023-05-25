// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleBrowser
{
	using System;
	using System.Collections.Generic;
    using System.Linq;

    using ExampleLibrary;
	using Gtk;

    #if GTK3
    using TreeModel = Gtk.ITreeModel;
    #endif
#if GTKSHARP4
    using Gio;
#endif

    public partial class MainWindow : Window
    {
#if GTKSHARP4
        Paned paned = Paned.New(Orientation.Horizontal);
        // ListView treeView;
#else
        HPaned paned = new HPaned ();
        // TreeView treeView;
#endif
        TreeView treeView;
        OxyPlot.GtkSharp.PlotView plotView;
        ExampleInfo selectedExample;

        public MainWindow()
#if !GTKSHARP4
            : base("Example Browser")
#endif
        {
#if GTKSHARP4
            this.Title = "Example Browser";
#endif
            this.Examples = ExampleLibrary.Examples.GetList().OrderBy(e => e.Category).ToList();
            this.InitializeComponent();
            this.SelectedExample = this.Examples.FirstOrDefault();
        }

        public IList<ExampleInfo> Examples { get; private set; }

        public ExampleInfo SelectedExample
        {
            get
            {
                return this.selectedExample;
            }

            set
            {
                this.selectedExample = value;
                this.plotView.Model = this.selectedExample != null ? this.selectedExample.PlotModel : null;
                this.plotView.Controller = this.selectedExample != null ? this.selectedExample.PlotController : null;
            }
        }

#if GTKSHARP4
        private class ModelClass : TreeStore
        {
            public ModelClass() : base(Gtk.Internal.TreeStore.Newv(2, new nuint[2] { GLib.Internal.String.GetGType(), GLib.Internal.String.GetGType() }), true)
            {
            }
        }
#endif

        private void InitializeComponent()
        {
            this.plotView = new OxyPlot.GtkSharp.PlotView();
            this.plotView.SetSizeRequest(300, 300);

#if GTKSHARP4
            // this.treeView = new ListView();
#else
            // this.treeView = new TreeView();
#endif
            this.treeView = new TreeView();
            this.treeView.Visible = true;

#if GTKSHARP4
            // ListModel root = StringList.New(this.Examples.Select(e => e.Title).ToArray());
            // // TRUE to pass through items from the models.
            // bool passthrough = false;
            // // TRUE to set the autoexpand property and expand the root model.
            // bool autoexpand = false;
            // // Function to call to create the GListModel for the children of an item.
			// var treeModel = TreeListModel.New(root, passthrough, autoexpand, CreateChildren);
            // SelectionModel selectionModel = SingleSelection.New(treeModel);
            // this.treeView.Model = selectionModel;
            nuint stringGType = GLib.Internal.String.GetGType();
            
            var treeModel = new ModelClass();
#else
            var treeModel = new TreeStore(typeof(string), typeof(string));
            var iter = new TreeIter();
#endif
            string last = null;
            foreach (var ex in this.Examples)
            {
                if (last == null || last != ex.Category)
                {
#if GTKSHARP4
                    // Gtk.Internal.TreeStore.Append()
#else
                    iter = treeModel.AppendValues(ex.Category);
#endif
                    last = ex.Category;
                }

                treeModel.AppendValues(iter, ex.Title, ex.Category);
            }
            this.treeView.Model = treeModel;
            var exampleNameColumn = new TreeViewColumn { Title = "Example" };
            var exampleNameCell = new CellRendererText();
            exampleNameColumn.PackStart(exampleNameCell, true);
            this.treeView.AppendColumn(exampleNameColumn);
            exampleNameColumn.AddAttribute(exampleNameCell, "text", 0);

            this.treeView.Selection.Changed += (s, e) =>
            {
                if (treeView.Selection.GetSelected(out var selectedModel, out var selectedNode))
                {
                    string val1 = (string)selectedModel.GetValue(selectedNode, 0);
                    string val2 = (string)selectedModel.GetValue(selectedNode, 1);

                    this.SelectedExample = this.Examples.FirstOrDefault(ex => ex.Category == val2 && ex.Title == val1);
                }
            };
#endif

            var scrollwin = new ScrolledWindow ();
#if GTKSHARP4
            scrollwin.Child = this.treeView;
#else
            scrollwin.Add (this.treeView);
#endif
            scrollwin.SetSizeRequest(250, 300);

            var txtSearch = new Entry ();
#if GTKSHARP4
            // TBI - ListView search.
#else
            treeView.SearchEntry = txtSearch;
#endif

            const int border = 6;
            this.paned.Position = 300;

#if GTKSHARP4
            var treeVbox = Box.New(Orientation.Vertical, 0);
            treeVbox.MarginBottom = border;
            treeVbox.MarginTop = border;
            treeVbox.MarginEnd = border;
            treeVbox.MarginStart = border;

            treeVbox.Append(txtSearch);
            treeVbox.Append(scrollwin);
            this.paned.StartChild = treeVbox;
            this.paned.EndChild = this.plotView;
            this.Child = this.paned;
#else
            var treeVbox = new VBox (false, 0);
            treeVbox.BorderWidth = border;
            treeVbox.PackStart (txtSearch, false, true, 0);
            treeVbox.PackStart (scrollwin, true, true, 0);
            this.paned.Pack1 (treeVbox, false, false);
            this.paned.Pack2 (this.plotView, true, false);
            this.Add(this.paned);
#endif

            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = Gtk.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(943, 554);
            this.Title = "OxyPlot.GtkSharp Example Browser";
#if !GTKSHARP4
            this.DeleteEvent += (s, a) =>
            {
                Application.Quit();
                a.RetVal = true;
            };
#endif
        }

#if GTKSHARP4
		private ListModel CreateChildren(GObject.Object item)
		{
			return StringList.New(new string[] { this.Examples.First().Category });
		}
#endif
	}
}