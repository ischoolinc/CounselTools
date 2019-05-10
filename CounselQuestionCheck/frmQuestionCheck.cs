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

        Dictionary<string, Dictionary<string, Dictionary<string, QuestionText>>> sourceDict = new Dictionary<string, Dictionary<string, Dictionary<string, QuestionText>>>();

        public frmQuestionCheck()
        {
            InitializeComponent();

        }


        private void LoadSourceData()
        {
            // 填入畫面
            advTree1.Nodes.Clear();
            if (subjectInfoList != null)
            {
                foreach (SubjectInfo subjInfo in subjectInfoList)
                {
                    Node n1 = new Node();

                    //for( int i = 1; i<= 10; i++)
                    //{
                    //    Cell c1 = new Cell();
                    //    c1.Text = "abc";
                    //    n1.Cells.Add(c1);
                    //}


                    foreach (QuestionGroup qg in subjInfo.QuestionGroupList)
                    {
                        Node n2 = new Node();
                        n2.Text = qg.Group;
                        foreach (QuestionQuery qq in qg.QuestionQueryList)
                        {
                            Node n3 = new Node();
                            n3.Text = qq.Query;

                            foreach (QuestionText qt in qq.QuestionTextList)
                            {
                                Node n4 = new Node();
                                n4.Text = "QuestionText";
                                n4.Tag = qt.QuestionCode;
                                // 5 個 cells

                                Cell c41 = new Cell();
                                c41.Text = "QuestionCode:" + qt.QuestionCode;
                                c41.Tag = qt.QuestionCode;
                                n4.Cells.Add(c41);
                                Cell c42 = new Cell();
                                c42.Text = "Type:" + qt.Type;
                                c42.Tag = qt.Type;
                                n4.Cells.Add(c42);
                                Cell c43 = new Cell();
                                c43.Text = "Require:" + qt.Require;
                                c43.Tag = qt.Require;
                                n4.Cells.Add(c43);
                                Cell c44 = new Cell();
                                c44.Text = "RequireLink:" + qt.RequireLink;
                                c44.Tag = qt.RequireLink;
                                n4.Cells.Add(c44);
                                Cell c45 = new Cell();
                                c45.Text = "Text:" + qt.Text;
                                c45.Tag = qt.Text;
                                n4.Cells.Add(c45);

                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                {
                                    Node n5 = new Node();
                                    n5.Text = "QuestionOptions";
                                    n5.Tag = qo.OptionCode;

                                    Cell c51 = new Cell();
                                    c51.Text = "AnswerID:" + qo.AnswerID;
                                    c51.Tag = qo.AnswerID;
                                    n5.Cells.Add(c51);

                                    Cell c52 = new Cell();
                                    c52.Text = "OptionCode:" + qo.OptionCode;
                                    c52.Tag = qo.OptionCode;
                                    n5.Cells.Add(c52);

                                    Cell c53 = new Cell();
                                    c53.Text = "OptionText:" + qo.OptionText;
                                    c53.Tag = qo.OptionText;
                                    n5.Cells.Add(c53);

                                    Cell c54 = new Cell();
                                    c54.Text = "AnswerValue:" + qo.AnswerValue;
                                    c54.Tag = qo.AnswerValue;
                                    n5.Cells.Add(c54);

                                    Cell c55 = new Cell();
                                    c55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                    c55.Tag = qo.AnswerMatrix;
                                    n5.Cells.Add(c55);

                                    Cell c56 = new Cell();
                                    c56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                    c56.Tag = qo.AnswerChecked;
                                    n5.Cells.Add(c56);

                                    Cell c57 = new Cell();
                                    c57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                    c57.Tag = qo.AnswerComplete;
                                    n5.Cells.Add(c57);

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


        private void frmQuestionCheck_Load(object sender, EventArgs e)
        {
            advTree1.Nodes.Clear();
            try
            {
                string sourcePath = Application.StartupPath + "\\question_template.json";

                JsonParser jp = new JsonParser();

                subjectInfoList = jp.LoadJSonFileAndParse(sourcePath);

                sourceDict.Clear();
                foreach (SubjectInfo ss in subjectInfoList)
                {
                    if (!sourceDict.ContainsKey(ss.Subject))
                        sourceDict.Add(ss.Subject, new Dictionary<string, Dictionary<string, QuestionText>>());

                    foreach (QuestionGroup qg in ss.QuestionGroupList)
                    {
                        if (!sourceDict[ss.Subject].ContainsKey(qg.Group))
                        {
                            sourceDict[ss.Subject].Add(qg.Group, new Dictionary<string, QuestionText>());
                        }

                        foreach (QuestionQuery qq in qg.QuestionQueryList)
                        {
                            if (!sourceDict[ss.Subject][qg.Group].ContainsKey(qq.Query))
                                sourceDict[ss.Subject][qg.Group].Add(qq.Query, new QuestionText());
                        }
                    }
                }

                // Console.WriteLine(subjectInfoList.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("來源樣板 question_template.json 解析 JSON 發生失敗," + ex.Message);
            }


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON Files (.json)|*.json|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadSourceData();

                JsonParser jp = new JsonParser();
                List<SubjectInfo> TargetSubjectList = jp.LoadJSonFileAndParse(ofd.FileName);
                //Console.Write(TargetSubjectList.Count);
                CheckData(TargetSubjectList);
                //    MessageBox.Show("完成");

            }
        }

        private void CheckData(List<SubjectInfo> targetSubjectList)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<QuestionText>>>> targetDict = new Dictionary<string, Dictionary<string, Dictionary<string, List<QuestionText>>>>();
            foreach (SubjectInfo ss in targetSubjectList)
            {
                if (!targetDict.ContainsKey(ss.Subject))
                    targetDict.Add(ss.Subject, new Dictionary<string, Dictionary<string, List<QuestionText>>>());

                foreach (QuestionGroup qg in ss.QuestionGroupList)
                {
                    if (!targetDict[ss.Subject].ContainsKey(qg.Group))
                    {
                        targetDict[ss.Subject].Add(qg.Group, new Dictionary<string, List<QuestionText>>());
                    }

                    foreach (QuestionQuery qq in qg.QuestionQueryList)
                    {
                        if (!targetDict[ss.Subject][qg.Group].ContainsKey(qq.Query))
                            targetDict[ss.Subject][qg.Group].Add(qq.Query, new List<QuestionText>());

                        targetDict[ss.Subject][qg.Group][qq.Query].AddRange(qq.QuestionTextList);
                    }
                }
            }

            //  檢查原始檔 Subject,Group,Query 在比對檔案是否存在，不存在用紅色表示
            foreach (Node item1 in advTree1.Nodes)
            {
                // subject
                if (targetDict.ContainsKey(item1.Text))
                {
                    foreach (Node item2 in item1.Nodes)
                    {
                        if (targetDict[item1.Text].ContainsKey(item2.Text))
                        {
                            foreach (Node item3 in item2.Nodes)
                            {
                                if (targetDict[item1.Text][item2.Text].ContainsKey(item3.Text))
                                {
                                    List<QuestionText> qtList = targetDict[item1.Text][item2.Text][item3.Text];
                                    // 比對 QuestionText:QuestionCode
                                    foreach (Node item4 in item3.Nodes)
                                    {
                                        bool item4Pass = false;
                                        string qtQuestionCode = item4.Tag.ToString();
                                        foreach (QuestionText qt in qtList)
                                        {
                                            if (qt.QuestionCode == qtQuestionCode)
                                            {
                                                item4Pass = true;
                                                foreach (Node item5 in item4.Nodes)
                                                {
                                                    bool item5Pass = false;
                                                    string QuestionOptionCode = item5.Tag.ToString();
                                                    // 再比對 Options
                                                    foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                                    {
                                                        if (qo.OptionCode == QuestionOptionCode)
                                                        {
                                                            item5Pass = true;

                                                        }
                                                    }
                                                    if (item5Pass == false)
                                                    {
                                                        item5.Style = new DevComponents.DotNetBar.ElementStyle();
                                                        item5.Style.TextColor = Color.Red;
                                                        item5.Parent.Expand();
                                                        item5.Parent.Parent.Expand();
                                                        item5.Parent.Parent.Parent.Expand();
                                                        item5.Parent.Parent.Parent.Parent.Expand();
                                                    }

                                                }
                                            }
                                        }

                                        if (item4Pass == false)
                                        {
                                            item4.Style = new DevComponents.DotNetBar.ElementStyle();
                                            item4.Style.TextColor = Color.Red;
                                            item4.Parent.Expand();
                                            item4.Parent.Parent.Expand();
                                            item4.Parent.Parent.Parent.Expand();
                                        }
                                    }
                                }
                                else
                                {
                                    item3.Style = new DevComponents.DotNetBar.ElementStyle();
                                    item3.Style.TextColor = Color.Red;
                                    item3.Expand();
                                    item3.Parent.Expand();
                                    item3.Parent.Parent.Expand();
                                }
                            }

                        }
                        else
                        {
                            item2.Style = new DevComponents.DotNetBar.ElementStyle();
                            item2.Style.TextColor = Color.Red;
                            item2.Expand();
                            item2.Parent.Expand();
                        }
                    }
                }
                else
                {
                    item1.Style = new DevComponents.DotNetBar.ElementStyle();
                    item1.Style.TextColor = Color.Red;
                    item1.Expand();
                }
            }

            Node errNodes = new Node();
            errNodes.Text = "比對檔案內有，來源檔案沒有";
            errNodes.Style = new DevComponents.DotNetBar.ElementStyle();
            errNodes.Style.TextColor = Color.Green;
            // 檢查比對檔有 Subject,Group,Query 來源沒有直接新增到 tree
            foreach (string key1 in targetDict.Keys)
            {

                Node HasN1 = new Node();
                HasN1.Text = key1;
                HasN1.Style = new DevComponents.DotNetBar.ElementStyle();
                HasN1.Style.TextColor = Color.Green;

                if (sourceDict.ContainsKey(key1))
                {
                    foreach (string key2 in targetDict[key1].Keys)
                    {
                        Node n2 = new Node();
                        n2.Text = key2;
                        n2.Style = new DevComponents.DotNetBar.ElementStyle();
                        n2.Style.TextColor = Color.Green;

                        if (sourceDict[key1].ContainsKey(key2))
                        {
                            foreach (string key3 in targetDict[key1][key2].Keys)
                            {
                                Node n3 = new Node();
                                n3.Text = key3;
                                n3.Style = new DevComponents.DotNetBar.ElementStyle();
                                n3.Style.TextColor = Color.Green;
                                if (!sourceDict[key1][key2].ContainsKey(key3))
                                {
                                    n2.Nodes.Add(n3);

                                }
                            }
                            if (n2.Nodes.Count > 0)
                                HasN1.Nodes.Add(n2);
                        }
                        else
                        {
                            HasN1.Nodes.Add(n2);
                        }
                    }
                    if (HasN1.Nodes.Count > 0)
                    {
                        errNodes.Nodes.Add(HasN1);
                    }
                }
                else
                {
                    // 沒有 subject
                    Node n1 = new Node();
                    n1.Text = key1;
                    n1.Style = new DevComponents.DotNetBar.ElementStyle();
                    n1.Style.TextColor = Color.Green;

                    foreach (string key2 in targetDict[key1].Keys)
                    {
                        Node n2 = new Node();
                        n2.Text = key2;
                        n2.Style = new DevComponents.DotNetBar.ElementStyle();
                        n2.Style.TextColor = Color.Green;

                        foreach (string key3 in targetDict[key1][key2].Keys)
                        {
                            Node n3 = new Node();
                            n3.Text = key3;
                            n3.Style = new DevComponents.DotNetBar.ElementStyle();
                            n3.Style.TextColor = Color.Green;
                            n2.Nodes.Add(n3);
                        }

                        n1.Nodes.Add(n2);
                    }

                    errNodes.Nodes.Add(n1);
                }
            }
            if (errNodes.Nodes.Count > 0)
            {
                advTree1.Nodes.Add(errNodes);
            }
        }

        private void advTree1_Click(object sender, EventArgs e)
        {

        }
    }

}

