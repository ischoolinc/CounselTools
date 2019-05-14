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
                    n1.Text = subjInfo.Subject;
                    n1.Tag = subjInfo;

                    foreach (QuestionGroup qg in subjInfo.QuestionGroupList)
                    {
                        Node n2 = new Node();
                        n2.Text = qg.Group;
                        n2.Tag = qg;

                        foreach (QuestionQuery qq in qg.QuestionQueryList)
                        {
                            Node n3 = new Node();
                            n3.Text = qq.Query;
                            n3.Tag = qq;
                            foreach (QuestionText qt in qq.QuestionTextList)
                            {
                                Node n4 = new Node();
                                n4.Text = "QuestionText";
                                n4.Tag = qt;
                                // 5 個 cells

                                Node cn41 = new Node();
                                cn41.Text = "QuestionCode:" + qt.QuestionCode;
                                cn41.Tag = "QuestionCode";

                                n4.Nodes.Add(cn41);
                                Node cn42 = new Node();
                                cn42.Text = "Type:" + qt.Type;
                                cn42.Tag = "Type";
                                n4.Nodes.Add(cn42);
                                Node cn43 = new Node();
                                cn43.Text = "Require:" + qt.Require;
                                cn43.Tag = "Require";

                                n4.Nodes.Add(cn43);
                                Node cn44 = new Node();
                                cn44.Text = "RequireLink:" + qt.RequireLink;
                                cn44.Tag = "RequireLink";
                                n4.Nodes.Add(cn44);
                                Node cn45 = new Node();
                                cn45.Text = "Text:" + qt.Text;
                                cn45.Tag = "Text";
                                n4.Nodes.Add(cn45);

                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                {
                                    Node n5 = new Node();
                                    n5.Text = "QuestionOptions";
                                    n5.Tag = qo;

                                    Node cn51 = new Node();
                                    cn51.Text = "AnswerID:" + qo.AnswerID;
                                    cn51.Tag = "AnswerID";
                                    n5.Nodes.Add(cn51);

                                    Node cn52 = new Node();
                                    cn52.Text = "OptionCode:" + qo.OptionCode;
                                    cn52.Tag = "OptionCode";
                                    n5.Nodes.Add(cn52);

                                    Node cn53 = new Node();
                                    cn53.Text = "OptionText:" + qo.OptionText;
                                    cn53.Tag = "OptionText";
                                    n5.Nodes.Add(cn53);

                                    Node cn54 = new Node();
                                    cn54.Text = "AnswerValue:" + qo.AnswerValue;
                                    cn54.Tag = "AnswerValue";
                                    n5.Nodes.Add(cn54);

                                    Node cn55 = new Node();
                                    cn55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                    cn55.Tag = "AnswerMatrix";
                                    n5.Nodes.Add(cn55);

                                    Node cn56 = new Node();
                                    cn56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                    cn56.Tag = "AnswerChecked";
                                    n5.Nodes.Add(cn56);

                                    Node cn57 = new Node();
                                    cn57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                    cn57.Tag = "AnswerComplete";
                                    n5.Nodes.Add(cn57);

                                    n4.Nodes.Add(n5);
                                }
                                n3.Nodes.Add(n4);
                            }

                            n2.Nodes.Add(n3);

                        }

                        n1.Nodes.Add(n2);
                    }
                    advTree1.Nodes.Add(n1);

                }
            }

            // 處理錯誤顏色標示
            foreach (Node n1 in advTree1.Nodes)
            {
                SubjectInfo sn1 = n1.Tag as SubjectInfo;
                if (sn1 != null)
                {
                    if (sn1.isSubjectPass == false)
                    {
                        n1.Style = new DevComponents.DotNetBar.ElementStyle();
                        n1.Style.TextColor = Color.Red;
                        n1.Expanded = true;
                    }
                }

                foreach (Node n2 in n1.Nodes)
                {
                    QuestionGroup qg = n2.Tag as QuestionGroup;
                    if (qg != null)
                    {
                        if (qg.isGroupPass == false)
                        {
                            n2.Style = new DevComponents.DotNetBar.ElementStyle();
                            n2.Style.TextColor = Color.Red;
                            n2.Expanded = true;
                            n2.Parent.Expanded = true;
                        }
                    }

                    foreach (Node n3 in n2.Nodes)
                    {
                        QuestionQuery qq = n3.Tag as QuestionQuery;
                        if (qq != null)
                        {
                            if (qq.isQueryPass == false)
                            {
                                n3.Style = new DevComponents.DotNetBar.ElementStyle();
                                n3.Style.TextColor = Color.Red;
                                n3.Expanded = true;
                                n3.Parent.Expanded = true;
                                n3.Parent.Parent.Expanded = true;
                            }

                            foreach (Node n4 in n3.Nodes)
                            {
                                QuestionText qt = n4.Tag as QuestionText;
                                if (qt != null)
                                {
                                    foreach (Node n4s in n4.Nodes)
                                    {
                                        string tagType = n4s.Tag.ToString();
                                        bool tagTypePass = true;
                                        switch (tagType)
                                        {
                                            case "Text":
                                                tagTypePass = qt.isTextPass;
                                                break;
                                            case "Type":
                                                tagTypePass = qt.isTypePass;
                                                break;
                                            case "Require":
                                                tagTypePass = qt.isRequirePass;
                                                break;
                                            case "RequireLink":

                                                tagTypePass = qt.isRequireLinkPass;
                                                break;
                                            case "QuestionCode":
                                                tagTypePass = qt.isQuestionCodePass;
                                                break;
                                        }

                                        if (tagTypePass == false)
                                        {
                                            n4s.Style = new DevComponents.DotNetBar.ElementStyle();
                                            n4s.Style.TextColor = Color.Red;
                                            n4s.Expanded = true;
                                            n4s.Parent.Expanded = true;
                                            n4s.Parent.Parent.Expanded = true;
                                            n4s.Parent.Parent.Parent.Expanded = true;
                                            n4s.Parent.Parent.Parent.Parent.Expanded = true;
                                        }

                                        foreach (Node n5 in n4.Nodes)
                                        {

                                            if (n5.Text == "QuestionOptions")
                                            {
                                               
                                                QuestionOptions qo = n5.Tag as QuestionOptions;

                                                if (qo != null)
                                                {
                                                    foreach (Node n5s in n5.Nodes)
                                                    {
                                                        string tagTypens5 = n5s.Tag.ToString();
                                                        bool tagTypePassn5 = true;
                                                        switch (tagTypens5)
                                                        {

                                                            case "AnswerID":
                                                                tagTypePassn5 = qo.isAnswerIDPass;
                                                                break;

                                                            case "OptionCode":
                                                                tagTypePassn5 = qo.isOptionCodePass;
                                                                break;

                                                            case "OptionText":
                                                                tagTypePassn5 = qo.isOptionTextPass;
                                                                break;

                                                            case "AnswerValue":
                                                                tagTypePassn5 = qo.isAnswerValuePass;
                                                                break;

                                                            case "AnswerMatrix":
                                                                tagTypePassn5 = qo.isAnswerMatrixPass;
                                                                break;

                                                            case "AnswerChecked":
                                                                tagTypePassn5 = qo.isAnswerCheckedPass;
                                                                break;

                                                            case "AnswerComplete":
                                                                tagTypePassn5 = qo.isAnswerCompletePass;
                                                                break;

                                                        }
                                                        if (tagTypePassn5 == false)
                                                        {
                                                            n5s.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            n5s.Style.TextColor = Color.Red;
                                                            n5s.Expanded = true;
                                                            n5s.Parent.Expanded = true;
                                                            n5s.Parent.Parent.Expanded = true;
                                                            n5s.Parent.Parent.Parent.Expanded = true;
                                                            n5s.Parent.Parent.Parent.Parent.Expanded = true;
                                                            n5s.Parent.Parent.Parent.Parent.Parent.Expanded = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }

                            }

                        }
                    }
                }
            }

        }


        private void frmQuestionCheck_Load(object sender, EventArgs e)
        {

            advTree1.Nodes.Clear();

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

                try
                {
                    advTree1.Nodes.Clear();

                    // 取得來源資料
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


                JsonParser jpT = new JsonParser();
                List<SubjectInfo> TargetSubjectList = jpT.LoadJSonFileAndParse(ofd.FileName);
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

            // 以來源為基礎比對
            foreach (SubjectInfo subject in subjectInfoList)
            {
                if (targetDict.ContainsKey(subject.Subject))
                {
                    subject.isSubjectPass = true;
                    foreach (QuestionGroup qg in subject.QuestionGroupList)
                    {
                        if (targetDict[subject.Subject].ContainsKey(qg.Group))
                        {
                            qg.isGroupPass = true;

                            foreach (QuestionQuery qq in qg.QuestionQueryList)
                            {
                                if (targetDict[subject.Subject][qg.Group].ContainsKey(qq.Query))
                                {
                                    qq.isQueryPass = true;

                                    List<QuestionText> targetQTList = targetDict[subject.Subject][qg.Group][qq.Query];

                                    // 以 QuestionText Text 當作 key 比對
                                    foreach (QuestionText qt in qq.QuestionTextList)
                                    {
                                        // 比對 QuestionText 上項目
                                        foreach (QuestionText tqt in targetQTList)
                                        {
                                            if (qt.Text == tqt.Text)
                                            {
                                                qt.isTextPass = true;

                                                if (qt.Require == tqt.Require)
                                                {
                                                    qt.isRequirePass = true;
                                                }
                                                if (qt.RequireLink == tqt.RequireLink)
                                                {
                                                    qt.isRequireLinkPass = true;
                                                }

                                                if (qt.Type == tqt.Type)
                                                {
                                                    qt.isTypePass = true;
                                                }

                                                if (qt.QuestionCode == tqt.QuestionCode)
                                                {
                                                    qt.isQuestionCodePass = true;
                                                }

                                                // 比對 Options
                                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                                {
                                                    foreach (QuestionOptions tqo in tqt.QuestionOptionsList)
                                                    {
                                                        if (qo.OptionText == tqo.OptionText)
                                                        {
                                                            qo.isOptionTextPass = true;

                                                            if (qo.AnswerID == tqo.AnswerID)
                                                                qo.isAnswerIDPass = true;

                                                            if (qo.OptionCode == tqo.OptionCode)
                                                                qo.isOptionCodePass = true;

                                                            if (qo.OptionText == tqo.OptionText)
                                                                qo.isOptionTextPass = true;

                                                            if (qo.AnswerValue == tqo.AnswerValue)
                                                                qo.isAnswerValuePass = true;

                                                            if (qo.AnswerMatrix == tqo.AnswerMatrix)
                                                                qo.isAnswerMatrixPass = true;

                                                            if (qo.AnswerChecked == tqo.AnswerChecked)
                                                                qo.isAnswerCheckedPass = true;

                                                            if (qo.AnswerComplete == tqo.AnswerComplete)
                                                                qo.isAnswerCompletePass = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    qq.isQueryPass = false;
                                }
                            }
                        }
                        else
                        {
                            qg.isGroupPass = false;
                        }
                    }
                }
                else
                {
                    subject.isSubjectPass = false;
                }
            }




            // 比對後放入 Tree
            LoadSourceData();



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

