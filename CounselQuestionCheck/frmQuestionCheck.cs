using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.AdvTree;
using DevComponents.DotNetBar.Controls;
using System.Web.Script.Serialization;
using System.IO;
using CounselQuestionCheck.DAO;


namespace CounselQuestionCheck
{
    public partial class frmQuestionCheck : FISCA.Presentation.Controls.BaseForm
    {
        List<SubjectInfo> subjectInfoList;

        public frmQuestionCheck()
        {
            InitializeComponent();
            
        }        

        private void frmQuestionCheck_Load(object sender, EventArgs e)
        {           
            try
            {
                string sourcePath = Application.StartupPath + "\\question190201v2.json";

                JsonParser jp = new JsonParser();
                
                subjectInfoList = jp.LoadJSonFileAndParse(sourcePath);

               // Console.WriteLine(subjectInfoList.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("解析 JSON 發生失敗," + ex.Message);
            }
            
            // 填入畫面
            advTree1.Nodes.Clear();
            if (subjectInfoList != null)
            {
                foreach (SubjectInfo subjInfo in subjectInfoList)
                {
                    Node n1 = new Node();

                    foreach(QuestionGroup qg in subjInfo.QuestionGroupList)
                    {
                        Node n2 = new Node();
                        n2.Text = qg.Group;
                        foreach(QuestionQuery qq in qg.QuestionQueryList)
                        {
                            Node n3 = new Node();
                            n3.Text = qq.Query;

                            foreach(QuestionText qt in qq.QuestionTextList)
                            {
                                Node n4 = new Node();
                                n4.Text = "QuestionText";

                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                {
                                    Node n5 = new Node();
                                    n5.Text = "QuestionOptions";
                                    n4.Nodes.Add(n5);
                                }
                                n3.Nodes.Add(n4);
                            }

                            n2.Nodes.Add(n3);
                        }

                        n1.Nodes.Add(n2);
                    }

                    n1.Style = new DevComponents.DotNetBar.ElementStyle();
                    //n1.Style.TextColor = Color.Blue;
                    //n1.Style.BackColor = Color.Red;
                    n1.Text = subjInfo.Subject;
                    n1.Tag = subjInfo;

                    advTree1.Nodes.Add(n1);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog () == DialogResult.OK)
            {
                JsonParser jp = new JsonParser();
                List<SubjectInfo> ss = jp.LoadJSonFileAndParse(ofd.FileName);
                Console.Write(ss.Count);
                   
            }
        }
    }

}

