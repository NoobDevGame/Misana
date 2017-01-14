﻿using Misana.Core.Maps;
using Misana.Editor.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Misana.Editor.Forms.MDI
{
    public partial class PropertyView : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.DockRight;

        private MainForm mainForm;

        public PropertyView(MainForm mainForm)
        {
            this.mainForm = mainForm;

            InitializeComponent();

            mainForm.EventBus.Subscribe<MapTileSelectionEvent>(t => SelectObjects<Tile>(t.SelectedTiles));
        }

        public void SelectObjects<T>(T[] objects) where T: class
        {
            propertyGrid.SelectedObjects = objects;
        }
    }
}