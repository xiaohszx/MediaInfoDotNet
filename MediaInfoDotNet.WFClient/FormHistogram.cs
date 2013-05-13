﻿/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * Histogramm of used tags 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaInfoDotNet.WFClient
{
	public partial class FormHistogram : Form
	{
		private BindingList<MediaFile> bindingList;
		protected struct args
		{
			public string category;
			public string keyword;
		}
		private FormHistogram() {
			InitializeComponent();
		}

		public FormHistogram(BindingList<MediaFile> bindingList)
			: this() {
			this.bindingList = bindingList;
		}

		private void tabKeywordsBindingNavigatorSaveItem_Click(object sender, EventArgs e) {
			if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				dataSetHistogram.tabKeywords.WriteXml(saveFileDialog1.FileName);
			}
		}
		private void tabKeywordsBindingNavigatorLoadItem_Click(object sender, EventArgs e) {
			if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				dataSetHistogram.tabKeywords.Clear();
				dataSetHistogram.tabKeywords.ReadXml(openFileDialog1.FileName);
			}
		}

		private void tabKeywordsBindingSource_CurrentChanged(object sender, EventArgs e) {

		}

		private void toolStripButtonStartAnalysis_Click(object sender, EventArgs e) {
			this.tabKeywordsBindingSource.DataSource = null;
			//this.tabKeywordsBindingSource.DataMember = "tabKeywords";
			backgroundWorker1.RunWorkerAsync();
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
			foreach (MediaFile mf in bindingList) {
				string info = mf.General.miInform();
				string[] lines = info.Split('\n');
				args progressarg;
				progressarg.category = "";
				progressarg.keyword = "";
				foreach (string line in lines) {
					if (!String.IsNullOrEmpty(line)) {
						if (line.IndexOf(':') > 0) {
							string[] keyword = line.Split(':');
							progressarg.keyword = keyword[0].Trim();
							backgroundWorker1.ReportProgress(0, (object)progressarg);
						}
						else {
							progressarg.category = line.Trim();
						}
					}
				}
			}
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			args arg = (args)e.UserState;
			var row = dataSetHistogram.tabKeywords.FindByKeyword(arg.keyword);
			if (row == null) {
				row = dataSetHistogram.tabKeywords.NewtabKeywordsRow();
				row.Keyword = arg.keyword;
				dataSetHistogram.tabKeywords.AddtabKeywordsRow(row);
			}

			row.BeginEdit();
			row.Keyword = arg.keyword;
			string cat2 = arg.category + "Hits";
			cat2 = cat2.Replace(" #10", "");
			cat2 = cat2.Replace(" #11", "");
			cat2 = cat2.Replace(" #12", "");
			cat2 = cat2.Replace(" #13", "");
			cat2 = cat2.Replace(" #14", "");
			cat2 = cat2.Replace(" #15", "");
			cat2 = cat2.Replace(" #1", "");
			cat2 = cat2.Replace(" #2", "");
			cat2 = cat2.Replace(" #3", "");
			cat2 = cat2.Replace(" #4", "");
			cat2 = cat2.Replace(" #5", "");
			cat2 = cat2.Replace(" #6", "");
			cat2 = cat2.Replace(" #7", "");
			cat2 = cat2.Replace(" #8", "");
			cat2 = cat2.Replace(" #9", "");
			if (row.Table.Columns.Contains(cat2))
				row[cat2] = (int)row[cat2] + 1;
			else
				System.Diagnostics.Debug.WriteLine("Booh");
			row.EndEdit();
			row.AcceptChanges();
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			System.Diagnostics.Debug.WriteLine("Booh");
			dataSetHistogram.tabKeywords.AcceptChanges();
			this.tabKeywordsBindingSource.DataSource = dataSetHistogram;
			this.tabKeywordsBindingSource.DataMember = "tabKeywords";
		}
	}
}