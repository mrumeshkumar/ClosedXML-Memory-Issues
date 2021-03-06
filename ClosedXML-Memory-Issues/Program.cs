﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace ClosedXML_Memory_Issues
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var wb = new XLWorkbook("MemoryIssuesTemplate.xlsx"))
            {
                
                using (var ws = wb.Worksheet("DATA"))
                {
                    var tbl = ws.Table("tblDATA");

                    var dt = new DataTable();
                    bool firstRow = true;

                    foreach(IXLRangeRow row in tbl.Rows())
                    {
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;

                            foreach(IXLCell cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }

                    //insert data several times - factor 5000 --> 50.000 rows
                    var dt2 = dt.Copy();
                    int factor = 5000;
                    for (int i = 1; i < factor; i++)
                    {
                        dt.Merge(dt2);
                    }


                    tbl.InsertRowsBelow(dt.Rows.Count);


                    tbl.DataRange.FirstCell().InsertData(dt.AsEnumerable().Skip(1));

                    tbl.Column(1).Style.NumberFormat.Format = "dd/mm/yyyy hh:mm";
                    tbl.Column(2).Style.NumberFormat.NumberFormatId = 14;

                    //hh:mm:ss
                    tbl.Column(3).Style.NumberFormat.NumberFormatId = 21;
                    tbl.Theme = XLTableTheme.TableStyleMedium2;

                    tbl.AutoFilter.Column(4).AddFilter("sda");


                    ws.Columns().AdjustToContents(1, 20);
                }
        
            wb.SaveAs("MemoryIssuesSaveAs.xlsx");
            }

            Environment.Exit(0);
        }
    }
}
