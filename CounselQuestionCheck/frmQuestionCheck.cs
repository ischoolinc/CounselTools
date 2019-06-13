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
using System.Data.SQLite;

namespace CounselQuestionCheck
{
    public partial class frmQuestionCheck : FISCA.Presentation.Controls.BaseForm
    {
        List<SubjectInfo> _subjectInfoList;
        List<SubjectInfo> _targetInfoList;

        ItemChecker _icr = new ItemChecker();
        SQLiteConnection cn;
        SQLiteCommand cmd;

        Dictionary<string, Dictionary<string, Dictionary<string, List<QuestionText>>>> sourceDict = new Dictionary<string, Dictionary<string, Dictionary<string, List<QuestionText>>>>();

        Dictionary<string, Dictionary<string, Dictionary<string, List<QuestionText>>>> targetDict = new Dictionary<string, Dictionary<string, Dictionary<string, List<QuestionText>>>>();

        string dbPath = @".\data.sqlite";
        string cnStr = "data source=data.sqlite";

        public frmQuestionCheck()
        {
            InitializeComponent();
        }


        private void frmQuestionCheck_Load(object sender, EventArgs e)
        {

            dbPath = Application.StartupPath + "\\data.sqlite";

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }


            cn = new SQLiteConnection(cnStr);
            cmd = new SQLiteCommand("", cn);
            cn.Open();

            //建立資料表
            string cmd_create_table = @"
CREATE TABLE IF NOT EXISTS source(
q_subject VARCHAR(1000),
q_group VARCHAR(1000),
q_query VARCHAR(1000),
q_text_QuestionCode varchar(1000),
q_text_Type varchar(1000),
q_text_Require varchar(1000),
q_text_RequireLink varchar(1000),
q_text_Text varchar(1000),
q_options_AnswerID  varchar(1000),
q_options_OptionCode  varchar(1000),
q_options_OptionText  varchar(1000),
q_options_AnswerValue  varchar(1000),
q_options_AnswerMatrix  varchar(1000),
q_options_AnswerChecked  varchar(1000),
q_options_AnswerComplete  varchar(1000),
qt_id varchar(100),
qo_id varchar(100)
);
CREATE TABLE IF NOT EXISTS target(
q_subject VARCHAR(1000),
q_group VARCHAR(1000),
q_query VARCHAR(1000),
q_text_QuestionCode varchar(1000),
q_text_Type varchar(1000),
q_text_Require varchar(1000),
q_text_RequireLink varchar(1000),
q_text_Text varchar(1000),
q_options_AnswerID  varchar(1000),
q_options_OptionCode  varchar(1000),
q_options_OptionText  varchar(1000),
q_options_AnswerValue  varchar(1000),
q_options_AnswerMatrix  varchar(1000),
q_options_AnswerChecked  varchar(1000),
q_options_AnswerComplete  varchar(1000),
qt_id varchar(100),
qo_id varchar(100)
);
";
            cmd.CommandText = cmd_create_table;
            cmd.ExecuteNonQuery();

            // cn.Close();

            advTree1.Nodes.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            btnLoadFile.Enabled = false;
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

                    _subjectInfoList = jp.LoadJSonFileAndParse(sourcePath);


                    SQLiteTransaction cnt = cn.BeginTransaction();

                    // 清空資料庫
                    string drop = "DELETE FROM source;";
                    cmd.CommandText = drop;
                    cmd.ExecuteNonQuery();
                    cnt.Commit();
                    cnt = cn.BeginTransaction();
                    // source 存入 sqlite
                    foreach (SubjectInfo ss in _subjectInfoList)
                    {
                        foreach (QuestionGroup qg in ss.QuestionGroupList)
                        {
                            foreach (QuestionQuery qq in qg.QuestionQueryList)
                            {
                                int qtCount = 0;
                                foreach (QuestionText qt in qq.QuestionTextList)
                                {
                                    qtCount++;
                                    string qt_id = "qt" + qtCount;

                                    if (qt.QuestionOptionsList.Count > 0)
                                    {
                                        int qoCount = 0;
                                        foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                        {
                                            qoCount++;
                                            string qo_id = "qo" + qoCount;

                                            string insertSQL = $"" +
    $"INSERT INTO source(" +
    $"q_subject," +
    $"q_group," +
    $"q_query," +
    $"q_text_QuestionCode," +
    $"q_text_Type," +
    $"q_text_Require," +
    $"q_text_RequireLink," +
    $"q_text_Text," +
    $"q_options_AnswerID ," +
    $"q_options_OptionCode ," +
    $"q_options_OptionText ," +
    $"q_options_AnswerValue ," +
    $"q_options_AnswerMatrix ," +
    $"q_options_AnswerChecked ," +
    $"q_options_AnswerComplete," +
    $"qt_id," +
    $"qo_id" +
    $")" +
    $" VALUES(" +
    $"'{ss.Subject}'" +
    $",'{qg.Group}'" +
    $",'{qq.Query}'" +
    $",'{qt.QuestionCode}'" +
    $",'{qt.Type}'" +
    $",'{qt.Require}'" +
    $",'{qt.RequireLink}'" +
    $",'{qt.Text}'" +
    $",'{qo.AnswerID}' " +
    $",'{qo.OptionCode}' " +
    $",'{qo.OptionText}' " +
    $",'{qo.AnswerValue}' " +
    $",'{qo.AnswerMatrix}' " +
    $",'{qo.AnswerChecked}' " +
    $",'{qo.AnswerComplete}'" +
    $",'{qt_id}'" +
    $",'{qo_id}'); " +
    $"";
                                            cmd.CommandText = insertSQL;
                                            cmd.ExecuteNonQuery();


                                        }

                                    }
                                    else
                                    {
                                        Console.Write("test");
                                    }

                                }

                            }
                        }
                    }

                    // target 存入 sqlite
                    cnt.Commit();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("來源樣板 question_template.json 解析 JSON 發生失敗," + ex.Message);
                    btnLoadFile.Enabled = true;
                }


                // 解析 target
                JsonParser jpT = new JsonParser();
                _targetInfoList = jpT.LoadJSonFileAndParse(ofd.FileName);
                //Console.Write(TargetSubjectList.Count);
                if (_subjectInfoList.Count > 0 && _targetInfoList.Count > 0)
                {
                    CheckData();
                }
                btnLoadFile.Enabled = true;
            }
            btnLoadFile.Enabled = true;
        }

        private void CheckData()
        {
            _icr = new ItemChecker();
            targetDict.Clear();

            SQLiteTransaction cnt = cn.BeginTransaction();

            // 清空資料庫
            string drop = "DELETE FROM target;";
            cmd.CommandText = drop;
            cmd.ExecuteNonQuery();
            cnt.Commit();
            cnt = cn.BeginTransaction();
            foreach (SubjectInfo ss in _targetInfoList)
            {
                foreach (QuestionGroup qg in ss.QuestionGroupList)
                {
                    foreach (QuestionQuery qq in qg.QuestionQueryList)
                    {
                        int qtCount = 0;
                        foreach (QuestionText qt in qq.QuestionTextList)
                        {
                            qtCount++;
                            string qt_id = "qt" + qtCount;

                            if (qt.QuestionOptionsList.Count > 0)
                            {
                                int qoCount = 0;
                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                {
                                    qoCount++;
                                    string qo_id = "qo" + qoCount;

                                    string insertSQL = $"" +
$"INSERT INTO target(" +
$"q_subject," +
$"q_group," +
$"q_query," +
$"q_text_QuestionCode," +
$"q_text_Type," +
$"q_text_Require," +
$"q_text_RequireLink," +
$"q_text_Text," +
$"q_options_AnswerID ," +
$"q_options_OptionCode ," +
$"q_options_OptionText ," +
$"q_options_AnswerValue ," +
$"q_options_AnswerMatrix ," +
$"q_options_AnswerChecked ," +
$"q_options_AnswerComplete," +
    $"qt_id," +
    $"qo_id" +
$")" +
$" VALUES(" +
$"'{ss.Subject}'" +
$",'{qg.Group}'" +
$",'{qq.Query}'" +
$",'{qt.QuestionCode}'" +
$",'{qt.Type}'" +
$",'{qt.Require}'" +
$",'{qt.RequireLink}'" +
$",'{qt.Text}'" +
$",'{qo.AnswerID}' " +
$",'{qo.OptionCode}' " +
$",'{qo.OptionText}' " +
$",'{qo.AnswerValue}' " +
$",'{qo.AnswerMatrix}' " +
$",'{qo.AnswerChecked}' " +
    $",'{qo.AnswerComplete}'" +
    $",'{qt_id}'" +
    $",'{qo_id}'); " +
$"";
                                    cmd.CommandText = insertSQL;
                                    cmd.ExecuteNonQuery();


                                }

                            }
                            else
                            {
                                Console.Write("test");
                            }

                        }

                    }
                }
            }
            cnt.Commit();

            // -- 改用 SQL 方式
            DataTable dt = new DataTable();
            string qry = @"
SELECT DISTINCT 
source.q_subject AS source
,target.q_subject AS target
,'source有target沒有_subject' AS message
,source.q_subject AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject
WHERE 
target.q_subject IS NULL
UNION
SELECT DISTINCT 
source.q_subject AS source
,target.q_subject AS target
,'source沒有target有_subject' AS message
,target.q_subject AS 'key'
,source.qt_id,source.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject
WHERE 
source.q_subject IS NULL
UNION
SELECT DISTINCT 
source.q_group AS source
,target.q_group AS target
,'source有target沒有_group' AS message
,source.q_subject ||'_'|| source.q_group AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group 
WHERE
target.q_group IS NULL
UNION
SELECT DISTINCT 
source.q_group AS source
,target.q_group AS target
,'source沒有target有_group' AS message
,target.q_subject ||'_'|| target.q_group AS 'key'
,source.qt_id,source.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group 
WHERE
source.q_group IS NULL
UNION
SELECT DISTINCT 
source.q_query AS source
,target.q_query AS target
,'source有target沒有_query' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query
WHERE
target.q_query IS NULL
UNION
SELECT DISTINCT 
source.q_query AS source
,target.q_query AS target
,'source沒有target有_query' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query AS 'key'
,source.qt_id,source.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query
WHERE
source.q_query IS NULL
UNION
SELECT DISTINCT 
source.q_text_QuestionCode AS source
,target.q_text_QuestionCode AS target
,'source有target沒有_q_text_QuestionCode' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_text_QuestionCode AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_QuestionCode = target.q_text_QuestionCode 
WHERE target.q_text_QuestionCode IS NULL
UNION
SELECT DISTINCT 
source.q_text_Type AS source
,target.q_text_Type AS target
,'source有target沒有_q_text_Type' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_text_Type AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_Type = target.q_text_Type 
WHERE target.q_text_Type IS NULL
UNION
SELECT DISTINCT 
source.q_text_Require AS source
,target.q_text_Require AS target
,'source有target沒有_q_text_Require' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_text_Require AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_Require = target.q_text_Require 
WHERE target.q_text_Require IS NULL
UNION
SELECT DISTINCT 
source.q_text_RequireLink AS source
,target.q_text_RequireLink AS target
,'source有target沒有_q_text_RequireLink' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_text_RequireLink AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_RequireLink = target.q_text_RequireLink 
WHERE target.q_text_RequireLink IS NULL
UNION
SELECT DISTINCT 
source.q_text_Text AS source
,target.q_text_Text AS target
,'source有target沒有_q_text_Text' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_text_Text AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_Text = target.q_text_Text 
WHERE target.q_text_Text IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerID AS source
,target.q_options_AnswerID AS target
,'source有target沒有_q_o_AnswerID' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_AnswerID AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerID = target.q_options_AnswerID 
WHERE target.q_options_AnswerID IS NULL
UNION
SELECT DISTINCT 
source.q_options_OptionCode AS source
,target.q_options_OptionCode AS target
,'source有target沒有_q_o_OptionCode' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_OptionCode AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_OptionCode = target.q_options_OptionCode 
WHERE target.q_options_OptionCode IS NULL
UNION
SELECT DISTINCT 
source.q_options_OptionText AS source
,target.q_options_OptionText AS target
,'source有target沒有_q_o_OptionText' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_OptionText AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_OptionText = target.q_options_OptionText 
WHERE target.q_options_OptionText IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerValue AS source
,target.q_options_AnswerValue AS target
,'source有target沒有_q_o_AnswerValue' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_AnswerValue AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerValue = target.q_options_AnswerValue 
WHERE target.q_options_AnswerValue IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerMatrix AS source
,target.q_options_AnswerMatrix AS target
,'source有target沒有_q_o_AnswerMatrix' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_AnswerMatrix AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerMatrix = target.q_options_AnswerMatrix 
WHERE target.q_options_AnswerMatrix IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerChecked AS source
,target.q_options_AnswerChecked AS target
,'source有target沒有_q_o_AnswerChecked' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_AnswerChecked AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerChecked = target.q_options_AnswerChecked 
WHERE target.q_options_AnswerChecked IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerComplete AS source
,target.q_options_AnswerComplete AS target
,'source有target沒有_q_o_AnswerComplete' AS message
,source.q_subject ||'_'|| source.q_group || '_' || source.q_query || '_' || source.q_options_AnswerComplete AS 'key'
,source.qt_id,source.qo_id
 FROM source LEFT JOIN target ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerComplete = target.q_options_AnswerComplete
WHERE target.q_options_AnswerComplete IS NULL
UNION
SELECT DISTINCT 
source.q_text_QuestionCode AS source
,target.q_text_QuestionCode AS target
,'source沒有target有_q_text_QuestionCode' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_text_QuestionCode AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_QuestionCode = target.q_text_QuestionCode 
WHERE source.q_text_QuestionCode IS NULL
UNION
SELECT DISTINCT 
source.q_text_Type AS source
,target.q_text_Type AS target
,'source沒有target有_q_text_Type' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_text_Type AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_Type = target.q_text_Type 
WHERE source.q_text_Type IS NULL
UNION
SELECT DISTINCT 
source.q_text_Require AS source
,target.q_text_Require AS target
,'source沒有target有_q_text_Require' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_text_Require AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_Require = target.q_text_Require 
WHERE source.q_text_Require IS NULL
UNION
SELECT DISTINCT 
source.q_text_RequireLink AS source
,target.q_text_RequireLink AS target
,'source沒有target有_q_text_RequireLink' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_text_RequireLink AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_RequireLink = target.q_text_RequireLink 
WHERE source.q_text_RequireLink IS NULL
UNION
SELECT DISTINCT 
source.q_text_Text AS source
,target.q_text_Text AS target
,'source沒有target有_q_text_Text' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_text_Text AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND 
source.q_text_Text = target.q_text_Text 
WHERE source.q_text_Text IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerID AS source
,target.q_options_AnswerID AS target
,'source沒有target有_q_o_AnswerID' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_AnswerID AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerID = target.q_options_AnswerID 
WHERE source.q_options_AnswerID IS NULL
UNION
SELECT DISTINCT 
source.q_options_OptionCode AS source
,target.q_options_OptionCode AS target
,'source沒有target有_q_o_OptionCode' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_OptionCode AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_OptionCode = target.q_options_OptionCode 
WHERE source.q_options_OptionCode IS NULL
UNION
SELECT DISTINCT 
source.q_options_OptionText AS source
,target.q_options_OptionText AS target
,'source沒有target有_q_o_OptionText' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_OptionText AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_OptionText = target.q_options_OptionText 
WHERE source.q_options_OptionText IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerValue AS source
,target.q_options_AnswerValue AS target
,'source沒有target有_q_o_AnswerValue' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_AnswerValue AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerValue = target.q_options_AnswerValue 
WHERE source.q_options_AnswerValue IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerMatrix AS source
,target.q_options_AnswerMatrix AS target
,'source沒有target有_q_o_AnswerMatrix' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_AnswerMatrix AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerMatrix = target.q_options_AnswerMatrix 
WHERE source.q_options_AnswerMatrix IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerMatrix AS source
,target.q_options_AnswerMatrix AS target
,'source沒有target有_q_o_AnswerMatrix' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_AnswerMatrix AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerMatrix = target.q_options_AnswerMatrix 
WHERE source.q_options_AnswerMatrix IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerChecked AS source
,target.q_options_AnswerChecked AS target
,'source沒有target有_q_o_AnswerChecked' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_AnswerChecked AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerChecked = target.q_options_AnswerChecked 
WHERE source.q_options_AnswerChecked IS NULL
UNION
SELECT DISTINCT 
source.q_options_AnswerComplete AS source
,target.q_options_AnswerComplete AS target
,'source沒有target有_q_o_AnswerComplete' AS message
,target.q_subject ||'_'|| target.q_group || '_' || target.q_query || '_' || target.q_options_AnswerComplete AS 'key'
,target.qt_id,target.qo_id
 FROM target LEFT JOIN source ON 
source.q_subject = target.q_subject AND
source.q_group = target.q_group AND
source.q_query = target.q_query AND
source.qt_id = target.qt_id AND source.qo_id = target.qo_id  AND  
source.q_options_AnswerComplete = target.q_options_AnswerComplete
WHERE source.q_options_AnswerComplete IS NULL


";
            SQLiteDataAdapter da = new SQLiteDataAdapter(qry, cn);
            da.Fill(dt);


            // 放入原始與比對資料放入資料 tree
            // 填入畫面
            advTree1.Nodes.Clear();
            if (_subjectInfoList != null)
            {

                // 來源先放入
                foreach (SubjectInfo subjInfo in _subjectInfoList)
                {
                    DataRow[] s1 = dt.Select("message = 'source有target沒有_subject' AND key = '" + subjInfo.Subject + "'");

                    Node n1 = new Node();
                    n1.Text = subjInfo.Subject;
                    n1.Tag = subjInfo;

                    if (s1 != null && s1.Count() > 0)
                    {
                        n1.Style = new DevComponents.DotNetBar.ElementStyle();
                        n1.Style.TextColor = Color.Red;
                        n1.Expand();
                    }

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

                            int qtCount = 0;
                            foreach (QuestionText qt in qq.QuestionTextList)
                            {
                                Node n4 = new Node();
                                n4.Text = "QuestionText";
                                n4.Tag = qt;
                                qtCount++;
                                string qt_id = "qt" + qtCount;
                                // 5 個 cells
                                qtCount++;
                                // 判斷是否展開 parent
                                bool checkExpendn4 = false;

                                Node cn41 = new Node();
                                cn41.Text = "QuestionCode:" + qt.QuestionCode;
                                cn41.Tag = "QuestionCode";
                                DataRow[] cn41s = dt.Select("message = 'source有target沒有_q_text_QuestionCode' AND qt_id = '" + qt_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.QuestionCode + "'");

                                if (cn41s != null && cn41s.Count() > 0)
                                {
                                    cn41.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn41.Style.TextColor = Color.Red;
                                    cn41.Expand();
                                    checkExpendn4 = true;
                                }

                                n4.Nodes.Add(cn41);

                                Node cn42 = new Node();
                                cn42.Text = "Type:" + qt.Type;
                                cn42.Tag = "Type";
                                DataRow[] cn42s = dt.Select("message = 'source有target沒有_q_text_Type' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.Type + "'");

                                if (cn42s != null && cn42s.Count() > 0)
                                {
                                    cn42.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn42.Style.TextColor = Color.Red;
                                    cn42.Expand();
                                    checkExpendn4 = true;

                                }
                                n4.Nodes.Add(cn42);


                                Node cn43 = new Node();
                                cn43.Text = "Require:" + qt.Require;
                                cn43.Tag = "Require";
                                DataRow[] cn43s = dt.Select("message = 'source有target沒有_q_text_Require'  AND qt_id = '" + qt_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.Require + "'");

                                if (cn43s != null && cn43s.Count() > 0)
                                {
                                    cn43.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn43.Style.TextColor = Color.Red;
                                    cn43.Expand();
                                    checkExpendn4 = true;
                                }

                                n4.Nodes.Add(cn43);

                                Node cn44 = new Node();
                                cn44.Text = "RequireLink:" + qt.RequireLink;
                                cn44.Tag = "RequireLink";
                                DataRow[] cn44s = dt.Select("message = 'source有target沒有_q_text_RequireLink'  AND qt_id = '" + qt_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.RequireLink + "'");

                                if (cn44s != null && cn44s.Count() > 0)
                                {
                                    cn44.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn44.Style.TextColor = Color.Red;
                                    cn44.Expand();
                                    checkExpendn4 = true;
                                }

                                n4.Nodes.Add(cn44);

                                Node cn45 = new Node();
                                cn45.Text = "Text:" + qt.Text;
                                cn45.Tag = "Text";
                                DataRow[] cn45s = dt.Select("message = 'source有target沒有_q_text_Text'  AND qt_id = '" + qt_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.Text + "'");

                                if (cn45s != null && cn45s.Count() > 0)
                                {
                                    cn45.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn45.Style.TextColor = Color.Red;
                                    cn45.Expand();
                                    checkExpendn4 = true;
                                }

                                n4.Nodes.Add(cn45);

                                // 確定需要展開 n4 parent
                                if (checkExpendn4)
                                {
                                    n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                    n4.Style.TextColor = Color.Red;
                                    n4.Expand();
                                    n3.Expand();
                                    n2.Expand();
                                    n1.Expand();
                                }

                                int qoCount = 0; bool checkAddn4 = false;
                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                {

                                    bool checkExpendn5 = false;

                                    Node n5 = new Node();
                                    n5.Text = "QuestionOptions";
                                    n5.Tag = qo;
                                    qoCount++;
                                    string qo_id = "qo" + qoCount;
                          

                                    Node cn51 = new Node();
                                    cn51.Text = "AnswerID:" + qo.AnswerID;
                                    cn51.Tag = "AnswerID";
                                    DataRow[] cn51s = dt.Select("message = 'source有target沒有_q_o_AnswerID' AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerID + "'");

                                    if (cn51s != null && cn51s.Count() > 0)
                                    {
                                        cn51.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn51.Style.TextColor = Color.Red;
                                        cn51.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn51);

                                    Node cn52 = new Node();
                                    cn52.Text = "OptionCode:" + qo.OptionCode;
                                    cn52.Tag = "OptionCode";
                                    DataRow[] cn52s = dt.Select("message = 'source有target沒有_q_o_OptionCode'  AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.OptionCode + "'");

                                    if (cn52s != null && cn52s.Count() > 0)
                                    {
                                        cn52.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn52.Style.TextColor = Color.Red;
                                        cn52.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn52);

                                    Node cn53 = new Node();
                                    cn53.Text = "OptionText:" + qo.OptionText;
                                    cn53.Tag = "OptionText";
                                    DataRow[] cn53s = dt.Select("message = 'source有target沒有_q_o_OptionText'  AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.OptionText + "'");

                                    if (cn53s != null && cn53s.Count() > 0)
                                    {
                                        cn53.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn53.Style.TextColor = Color.Red;
                                        cn53.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn53);

                                    Node cn54 = new Node();
                                    cn54.Text = "AnswerValue:" + qo.AnswerValue;
                                    cn54.Tag = "AnswerValue";
                                    DataRow[] cn54s = dt.Select("message = 'source有target沒有_q_o_AnswerValue'  AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerValue + "'");

                                    if (cn54s != null && cn54s.Count() > 0)
                                    {
                                        cn54.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn54.Style.TextColor = Color.Red;
                                        cn54.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn54);

                                    Node cn55 = new Node();
                                    cn55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                    cn55.Tag = "AnswerMatrix";
                                    DataRow[] cn55s = dt.Select("message = 'source有target沒有_q_o_AnswerMatrix'  AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerMatrix + "'");

                                    if (cn55s != null && cn55s.Count() > 0)
                                    {
                                        cn55.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn55.Style.TextColor = Color.Red;
                                        cn55.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn55);

                                    Node cn56 = new Node();
                                    cn56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                    cn56.Tag = "AnswerChecked";
                                    DataRow[] cn56s = dt.Select("message = 'source有target沒有_q_o_AnswerChecked'  AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerChecked + "'");

                                    if (cn56s != null && cn56s.Count() > 0)
                                    {
                                        cn56.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn56.Style.TextColor = Color.Red;
                                        cn56.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn56);

                                    Node cn57 = new Node();
                                    cn57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                    cn57.Tag = "AnswerComplete";
                                    DataRow[] cn57s = dt.Select("message = 'source有target沒有_q_o_AnswerComplete'  AND qo_id = '" + qo_id + "' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerComplete + "'");

                                    if (cn57s != null && cn57s.Count() > 0)
                                    {
                                        cn57.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn57.Style.TextColor = Color.Red;
                                        cn57.Expand();
                                        checkExpendn5 = true;
                                    }

                                    n5.Nodes.Add(cn57);

                                    if (checkExpendn5)
                                    {
                                        n5.Style = new DevComponents.DotNetBar.ElementStyle();
                                        n5.Style.TextColor = Color.Red;
                                        n5.Expand();
                                        n4.Expand();
                                        n3.Expand();
                                        n2.Expand();
                                        n1.Expand();
                                        checkAddn4 = true;
                                    }

                                    n4.Nodes.Add(n5);
                                }
                                                               

                                n3.Nodes.Add(n4);
                                if (checkExpendn4 == false && checkAddn4 == true)
                                {
                                    n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                    n4.Style.TextColor = Color.Red;
                                    n4.Expand();
                                    n3.Expand();
                                    n2.Expand();
                                    n1.Expand();
                                }
                            }

                            n2.Nodes.Add(n3);
                            DataRow[] s3 = dt.Select("message = 'source有target沒有_query' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "'");

                            if (s3 != null && s3.Count() > 0)
                            {
                                n3.Style = new DevComponents.DotNetBar.ElementStyle();
                                n3.Style.TextColor = Color.Red;
                                n3.Expand();
                                n2.Expand();
                                n1.Expand();
                            }
                        }

                        n1.Nodes.Add(n2);
                        DataRow[] s2 = dt.Select("message = 'source有target沒有_group' AND key = '" + subjInfo.Subject + "_" + qg.Group + "'");

                        if (s2 != null && s2.Count() > 0)
                        {
                            n2.Style = new DevComponents.DotNetBar.ElementStyle();
                            n2.Style.TextColor = Color.Red;
                            n2.Expand();
                            n1.Expand();
                        }
                    }
                    advTree1.Nodes.Add(n1);
                }

            }


            // 放入 target 沒有
            if (_targetInfoList != null)
            {
                // 第一層 subject 沒有
                foreach (SubjectInfo subjInfo in _targetInfoList)
                {
                    DataRow[] t1 = dt.Select("message = 'source沒有target有_subject' AND key = '" + subjInfo.Subject + "'");

                    if (t1 != null && t1.Count() > 0)
                    {

                        Node n1 = new Node();
                        n1.Text = subjInfo.Subject;
                        n1.Tag = subjInfo;
                        n1.Style = new DevComponents.DotNetBar.ElementStyle();
                        n1.Style.TextColor = Color.Gray;
                        n1.Expand();
                        foreach (QuestionGroup qg in subjInfo.QuestionGroupList)
                        {

                            Node n2 = new Node();
                            n2.Text = qg.Group;
                            n2.Tag = subjInfo;
                            n2.Style = new DevComponents.DotNetBar.ElementStyle();
                            n2.Style.TextColor = Color.Gray;
                            n2.Expand();
                            n1.Expand();

                            foreach (QuestionQuery qq in qg.QuestionQueryList)
                            {

                                Node n3 = new Node();
                                n3.Text = qq.Query;
                                n3.Tag = subjInfo;
                                n3.Style = new DevComponents.DotNetBar.ElementStyle();
                                n3.Style.TextColor = Color.Gray;
                                n2.Nodes.Add(n3);
                                n3.Expand();
                                n2.Expand();
                                n1.Expand();

                                foreach (QuestionText qt in qq.QuestionTextList)
                                {
                                    Node n4 = new Node();
                                    n4.Text = "QuestionText";
                                    n4.Tag = qt;
                                    n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                    n4.Style.TextColor = Color.Gray;
                                    n4.Expand();

                                    Node cn41 = new Node();
                                    cn41.Text = "QuestionCode:" + qt.QuestionCode;
                                    cn41.Tag = "QuestionCode";
                                    cn41.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn41.Style.TextColor = Color.Gray;
                                    cn41.Expand();

                                    n4.Nodes.Add(cn41);

                                    Node cn42 = new Node();
                                    cn42.Text = "Type:" + qt.Type;
                                    cn42.Tag = "Type";

                                    cn42.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn42.Style.TextColor = Color.Gray;
                                    cn42.Expand();

                                    n4.Nodes.Add(cn42);


                                    Node cn43 = new Node();
                                    cn43.Text = "Require:" + qt.Require;
                                    cn43.Tag = "Require";

                                    cn43.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn43.Style.TextColor = Color.Gray;
                                    cn43.Expand();

                                    n4.Nodes.Add(cn43);

                                    Node cn44 = new Node();
                                    cn44.Text = "RequireLink:" + qt.RequireLink;
                                    cn44.Tag = "RequireLink";
                                    cn44.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn44.Style.TextColor = Color.Gray;
                                    cn44.Expand();

                                    n4.Nodes.Add(cn44);

                                    Node cn45 = new Node();
                                    cn45.Text = "Text:" + qt.Text;
                                    cn45.Tag = "Text";

                                    cn45.Style = new DevComponents.DotNetBar.ElementStyle();
                                    cn45.Style.TextColor = Color.Gray;
                                    cn45.Expand();

                                    n4.Nodes.Add(cn45);

                                    foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                    {
                                        Node n5 = new Node();
                                        n5.Text = "QuestionOptions";
                                        n5.Tag = qo;
                                        n5.Style = new DevComponents.DotNetBar.ElementStyle();
                                        n5.Style.TextColor = Color.Gray;
                                        n5.Expand();

                                        Node cn51 = new Node();
                                        cn51.Text = "AnswerID:" + qo.AnswerID;
                                        cn51.Tag = "AnswerID";

                                        cn51.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn51.Style.TextColor = Color.Gray;
                                        cn51.Expand();

                                        n5.Nodes.Add(cn51);

                                        Node cn52 = new Node();
                                        cn52.Text = "OptionCode:" + qo.OptionCode;
                                        cn52.Tag = "OptionCode";

                                        cn52.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn52.Style.TextColor = Color.Gray;
                                        cn52.Expand();

                                        n5.Nodes.Add(cn52);

                                        Node cn53 = new Node();
                                        cn53.Text = "OptionText:" + qo.OptionText;
                                        cn53.Tag = "OptionText";

                                        cn53.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn53.Style.TextColor = Color.Gray;
                                        cn53.Expand();

                                        n5.Nodes.Add(cn53);

                                        Node cn54 = new Node();
                                        cn54.Text = "AnswerValue:" + qo.AnswerValue;
                                        cn54.Tag = "AnswerValue";

                                        cn54.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn54.Style.TextColor = Color.Gray;
                                        cn54.Expand();

                                        n5.Nodes.Add(cn54);

                                        Node cn55 = new Node();
                                        cn55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                        cn55.Tag = "AnswerMatrix";

                                        cn55.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn55.Style.TextColor = Color.Gray;
                                        cn55.Expand();

                                        n5.Nodes.Add(cn55);

                                        Node cn56 = new Node();
                                        cn56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                        cn56.Tag = "AnswerChecked";

                                        cn56.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn56.Style.TextColor = Color.Gray;
                                        cn56.Expand();

                                        n5.Nodes.Add(cn56);

                                        Node cn57 = new Node();
                                        cn57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                        cn57.Tag = "AnswerComplete";

                                        cn57.Style = new DevComponents.DotNetBar.ElementStyle();
                                        cn57.Style.TextColor = Color.Gray;
                                        cn57.Expand();

                                        n5.Nodes.Add(cn57);

                                        n4.Nodes.Add(n5);
                                    }
                                    n3.Nodes.Add(n4);
                                }

                            }

                            n1.Nodes.Add(n2);

                        }

                        advTree1.Nodes.Add(n1);
                    }
                    else
                    {

                        foreach (QuestionGroup qg in subjInfo.QuestionGroupList)
                        {
                            DataRow[] t2 = dt.Select("message = 'source沒有target有_group' AND key = '" + subjInfo.Subject + "_" + qg.Group + "'");

                            //  // 當 Subject 相同,將 Add 差異 Group
                            if (t2 != null && t2.Count() > 0)
                            {
                                foreach (Node n1 in advTree1.Nodes)
                                {
                                    if (n1.Text == subjInfo.Subject)
                                    {
                                        Node n2 = new Node();
                                        n2.Text = qg.Group;
                                        n2.Tag = subjInfo;
                                        n2.Style = new DevComponents.DotNetBar.ElementStyle();
                                        n2.Style.TextColor = Color.Gray;
                                        n2.Expand();
                                        n1.Expand();

                                        foreach (QuestionQuery qq in qg.QuestionQueryList)
                                        {

                                            Node n3 = new Node();
                                            n3.Text = qq.Query;
                                            n3.Tag = subjInfo;
                                            n3.Style = new DevComponents.DotNetBar.ElementStyle();
                                            n3.Style.TextColor = Color.Gray;
                                            n2.Nodes.Add(n3);
                                            n3.Expand();
                                            n2.Expand();
                                            n1.Expand();

                                            foreach (QuestionText qt in qq.QuestionTextList)
                                            {
                                                Node n4 = new Node();
                                                n4.Text = "QuestionText";
                                                n4.Tag = qt;
                                                n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                                n4.Style.TextColor = Color.Gray;
                                                n4.Expand();

                                                Node cn41 = new Node();
                                                cn41.Text = "QuestionCode:" + qt.QuestionCode;
                                                cn41.Tag = "QuestionCode";
                                                cn41.Style = new DevComponents.DotNetBar.ElementStyle();
                                                cn41.Style.TextColor = Color.Gray;
                                                cn41.Expand();

                                                n4.Nodes.Add(cn41);

                                                Node cn42 = new Node();
                                                cn42.Text = "Type:" + qt.Type;
                                                cn42.Tag = "Type";

                                                cn42.Style = new DevComponents.DotNetBar.ElementStyle();
                                                cn42.Style.TextColor = Color.Gray;
                                                cn42.Expand();

                                                n4.Nodes.Add(cn42);


                                                Node cn43 = new Node();
                                                cn43.Text = "Require:" + qt.Require;
                                                cn43.Tag = "Require";

                                                cn43.Style = new DevComponents.DotNetBar.ElementStyle();
                                                cn43.Style.TextColor = Color.Gray;
                                                cn43.Expand();

                                                n4.Nodes.Add(cn43);

                                                Node cn44 = new Node();
                                                cn44.Text = "RequireLink:" + qt.RequireLink;
                                                cn44.Tag = "RequireLink";
                                                cn44.Style = new DevComponents.DotNetBar.ElementStyle();
                                                cn44.Style.TextColor = Color.Gray;
                                                cn44.Expand();

                                                n4.Nodes.Add(cn44);

                                                Node cn45 = new Node();
                                                cn45.Text = "Text:" + qt.Text;
                                                cn45.Tag = "Text";

                                                cn45.Style = new DevComponents.DotNetBar.ElementStyle();
                                                cn45.Style.TextColor = Color.Gray;
                                                cn45.Expand();

                                                n4.Nodes.Add(cn45);

                                                foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                                {
                                                    Node n5 = new Node();
                                                    n5.Text = "QuestionOptions";
                                                    n5.Tag = qo;
                                                    n5.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    n5.Style.TextColor = Color.Gray;
                                                    n5.Expand();

                                                    Node cn51 = new Node();
                                                    cn51.Text = "AnswerID:" + qo.AnswerID;
                                                    cn51.Tag = "AnswerID";

                                                    cn51.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn51.Style.TextColor = Color.Gray;
                                                    cn51.Expand();

                                                    n5.Nodes.Add(cn51);

                                                    Node cn52 = new Node();
                                                    cn52.Text = "OptionCode:" + qo.OptionCode;
                                                    cn52.Tag = "OptionCode";

                                                    cn52.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn52.Style.TextColor = Color.Gray;
                                                    cn52.Expand();

                                                    n5.Nodes.Add(cn52);

                                                    Node cn53 = new Node();
                                                    cn53.Text = "OptionText:" + qo.OptionText;
                                                    cn53.Tag = "OptionText";

                                                    cn53.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn53.Style.TextColor = Color.Gray;
                                                    cn53.Expand();

                                                    n5.Nodes.Add(cn53);

                                                    Node cn54 = new Node();
                                                    cn54.Text = "AnswerValue:" + qo.AnswerValue;
                                                    cn54.Tag = "AnswerValue";

                                                    cn54.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn54.Style.TextColor = Color.Gray;
                                                    cn54.Expand();

                                                    n5.Nodes.Add(cn54);

                                                    Node cn55 = new Node();
                                                    cn55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                                    cn55.Tag = "AnswerMatrix";

                                                    cn55.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn55.Style.TextColor = Color.Gray;
                                                    cn55.Expand();

                                                    n5.Nodes.Add(cn55);

                                                    Node cn56 = new Node();
                                                    cn56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                                    cn56.Tag = "AnswerChecked";

                                                    cn56.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn56.Style.TextColor = Color.Gray;
                                                    cn56.Expand();

                                                    n5.Nodes.Add(cn56);

                                                    Node cn57 = new Node();
                                                    cn57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                                    cn57.Tag = "AnswerComplete";

                                                    cn57.Style = new DevComponents.DotNetBar.ElementStyle();
                                                    cn57.Style.TextColor = Color.Gray;
                                                    cn57.Expand();

                                                    n5.Nodes.Add(cn57);

                                                    n4.Nodes.Add(n5);
                                                }
                                                n3.Nodes.Add(n4);
                                            }

                                        }

                                        n1.Nodes.Add(n2);
                                    }
                                }
                            }
                            else
                            {
                                foreach (QuestionQuery qq in qg.QuestionQueryList)
                                {
                                    DataRow[] t3 = dt.Select("message = 'source沒有target有_query' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "'");

                                    if (t3 != null && t3.Count() > 0)
                                    {
                                        foreach (Node n1 in advTree1.Nodes)
                                        {
                                            if (n1.Text == subjInfo.Subject)
                                            {
                                                foreach (Node n2 in n1.Nodes)
                                                {
                                                    if (n2.Text == qg.Group)
                                                    {
                                                        Node n3 = new Node();
                                                        n3.Text = qq.Query;
                                                        n3.Tag = subjInfo;
                                                        n3.Style = new DevComponents.DotNetBar.ElementStyle();
                                                        n3.Style.TextColor = Color.Gray;
                                                        n2.Nodes.Add(n3);
                                                        n3.Expand();
                                                        n2.Expand();
                                                        n1.Expand();

                                                        foreach (QuestionText qt in qq.QuestionTextList)
                                                        {
                                                            Node n4 = new Node();
                                                            n4.Text = "QuestionText";
                                                            n4.Tag = qt;
                                                            n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            n4.Style.TextColor = Color.Gray;
                                                            n4.Expand();

                                                            Node cn41 = new Node();
                                                            cn41.Text = "QuestionCode:" + qt.QuestionCode;
                                                            cn41.Tag = "QuestionCode";
                                                            cn41.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            cn41.Style.TextColor = Color.Gray;
                                                            cn41.Expand();

                                                            n4.Nodes.Add(cn41);

                                                            Node cn42 = new Node();
                                                            cn42.Text = "Type:" + qt.Type;
                                                            cn42.Tag = "Type";

                                                            cn42.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            cn42.Style.TextColor = Color.Gray;
                                                            cn42.Expand();

                                                            n4.Nodes.Add(cn42);


                                                            Node cn43 = new Node();
                                                            cn43.Text = "Require:" + qt.Require;
                                                            cn43.Tag = "Require";

                                                            cn43.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            cn43.Style.TextColor = Color.Gray;
                                                            cn43.Expand();

                                                            n4.Nodes.Add(cn43);

                                                            Node cn44 = new Node();
                                                            cn44.Text = "RequireLink:" + qt.RequireLink;
                                                            cn44.Tag = "RequireLink";
                                                            cn44.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            cn44.Style.TextColor = Color.Gray;
                                                            cn44.Expand();

                                                            n4.Nodes.Add(cn44);

                                                            Node cn45 = new Node();
                                                            cn45.Text = "Text:" + qt.Text;
                                                            cn45.Tag = "Text";

                                                            cn45.Style = new DevComponents.DotNetBar.ElementStyle();
                                                            cn45.Style.TextColor = Color.Gray;
                                                            cn45.Expand();

                                                            n4.Nodes.Add(cn45);

                                                            foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                                            {
                                                                Node n5 = new Node();
                                                                n5.Text = "QuestionOptions";
                                                                n5.Tag = qo;
                                                                n5.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                n5.Style.TextColor = Color.Gray;
                                                                n5.Expand();

                                                                Node cn51 = new Node();
                                                                cn51.Text = "AnswerID:" + qo.AnswerID;
                                                                cn51.Tag = "AnswerID";

                                                                cn51.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn51.Style.TextColor = Color.Gray;
                                                                cn51.Expand();

                                                                n5.Nodes.Add(cn51);

                                                                Node cn52 = new Node();
                                                                cn52.Text = "OptionCode:" + qo.OptionCode;
                                                                cn52.Tag = "OptionCode";

                                                                cn52.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn52.Style.TextColor = Color.Gray;
                                                                cn52.Expand();

                                                                n5.Nodes.Add(cn52);

                                                                Node cn53 = new Node();
                                                                cn53.Text = "OptionText:" + qo.OptionText;
                                                                cn53.Tag = "OptionText";

                                                                cn53.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn53.Style.TextColor = Color.Gray;
                                                                cn53.Expand();

                                                                n5.Nodes.Add(cn53);

                                                                Node cn54 = new Node();
                                                                cn54.Text = "AnswerValue:" + qo.AnswerValue;
                                                                cn54.Tag = "AnswerValue";

                                                                cn54.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn54.Style.TextColor = Color.Gray;
                                                                cn54.Expand();

                                                                n5.Nodes.Add(cn54);

                                                                Node cn55 = new Node();
                                                                cn55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                                                cn55.Tag = "AnswerMatrix";

                                                                cn55.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn55.Style.TextColor = Color.Gray;
                                                                cn55.Expand();

                                                                n5.Nodes.Add(cn55);

                                                                Node cn56 = new Node();
                                                                cn56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                                                cn56.Tag = "AnswerChecked";

                                                                cn56.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn56.Style.TextColor = Color.Gray;
                                                                cn56.Expand();

                                                                n5.Nodes.Add(cn56);

                                                                Node cn57 = new Node();
                                                                cn57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                                                cn57.Tag = "AnswerComplete";

                                                                cn57.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                cn57.Style.TextColor = Color.Gray;
                                                                cn57.Expand();

                                                                n5.Nodes.Add(cn57);

                                                                n4.Nodes.Add(n5);
                                                            }
                                                            n3.Nodes.Add(n4);
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        // 當 query 相同
                                        foreach (Node n1 in advTree1.Nodes)
                                        {
                                            if (n1.Text == subjInfo.Subject)
                                            {
                                                foreach (Node n2 in n1.Nodes)
                                                {
                                                    if (n2.Text == qg.Group)
                                                    {
                                                        foreach (Node n3 in n2.Nodes)
                                                        {
                                                            if (n3.Text == qq.Query)
                                                            {
                                                                int qtCount = 0;
                                                                foreach (QuestionText qt in qq.QuestionTextList)
                                                                {
                                                                    qtCount++;
                                                                    string qt_id = "qt" + qtCount;
                                                                    Node n4 = new Node();
                                                                    n4.Text = "QuestionText";
                                                                    n4.Tag = qt;
                                                                    // 5 個 cells

                                                                    // 判斷是否展開 parent
                                                                    bool checkExpendn4 = false;


                                                                    DataRow[] cn41s = dt.Select("message = 'source沒有target有_q_text_QuestionCode' AND qt_id = '" + qt_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.QuestionCode + "'");
                                                                    if (cn41s != null && cn41s.Count() > 0)
                                                                    {
                                                                        Node cn41 = new Node();
                                                                        cn41.Text = "QuestionCode:" + qt.QuestionCode;
                                                                        cn41.Tag = "QuestionCode";
                                                                        cn41.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        cn41.Style.TextColor = Color.Gray;
                                                                        cn41.Expand();
                                                                        checkExpendn4 = true;
                                                                        n4.Nodes.Add(cn41);
                                                                    }


                                                                    DataRow[] cn42s = dt.Select("message = 'source沒有target有_q_text_Type'  AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.Type + "'");

                                                                    if (cn42s != null && cn42s.Count() > 0)
                                                                    {
                                                                        Node cn42 = new Node();
                                                                        cn42.Text = "Type:" + qt.Type;
                                                                        cn42.Tag = "Type";
                                                                        cn42.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        cn42.Style.TextColor = Color.Gray;
                                                                        cn42.Expand();
                                                                        checkExpendn4 = true;
                                                                        n4.Nodes.Add(cn42);
                                                                    }

                                                                    DataRow[] cn43s = dt.Select("message = 'source沒有target有_q_text_Require'  AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.Require + "'");

                                                                    if (cn43s != null && cn43s.Count() > 0)
                                                                    {
                                                                        Node cn43 = new Node();
                                                                        cn43.Text = "Require:" + qt.Require;
                                                                        cn43.Tag = "Require";
                                                                        cn43.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        cn43.Style.TextColor = Color.Gray;
                                                                        cn43.Expand();
                                                                        checkExpendn4 = true;
                                                                        n4.Nodes.Add(cn43);
                                                                    }


                                                                    DataRow[] cn44s = dt.Select("message = 'source沒有target有_q_text_RequireLink'  AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.RequireLink + "'");

                                                                    if (cn44s != null && cn44s.Count() > 0)
                                                                    {
                                                                        Node cn44 = new Node();
                                                                        cn44.Text = "RequireLink:" + qt.RequireLink;
                                                                        cn44.Tag = "RequireLink";
                                                                        cn44.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        cn44.Style.TextColor = Color.Gray;
                                                                        cn44.Expand();
                                                                        checkExpendn4 = true;
                                                                        n4.Nodes.Add(cn44);
                                                                    }

                                                                    DataRow[] cn45s = dt.Select("message = 'source沒有target有_q_text_Text' AND qt_id = '" + qt_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qt.Text + "'");

                                                                    if (cn45s != null && cn45s.Count() > 0)
                                                                    {
                                                                        Node cn45 = new Node();
                                                                        cn45.Text = "Text:" + qt.Text;
                                                                        cn45.Tag = "Text";
                                                                        cn45.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        cn45.Style.TextColor = Color.Gray;
                                                                        cn45.Expand();
                                                                        checkExpendn4 = true;
                                                                        n4.Nodes.Add(cn45);
                                                                    }

                                                                    // 確定需要展開 n4 parent
                                                                    if (checkExpendn4)
                                                                    {
                                                                        n3.Nodes.Add(n4);
                                                                        n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        n4.Style.TextColor = Color.Gray;
                                                                        n4.Expand();
                                                                        n3.Expand();
                                                                        n2.Expand();
                                                                        n1.Expand();
                                                                    }

                                                                    int qoCount = 0; bool checkAddN4 = false;
                                                                    foreach (QuestionOptions qo in qt.QuestionOptionsList)
                                                                    {
                                                                        qoCount++;
                                                                        bool checkExpendn5 = false;
                                                                        string qo_id = "qo" + qoCount;
                                                                        Node n5 = new Node();
                                                                        n5.Text = "QuestionOptions";
                                                                        n5.Tag = qo;


                                                                        DataRow[] cn51s = dt.Select("message = 'source沒有target有_q_o_AnswerID'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerID + "'");

                                                                        if (cn51s != null && cn51s.Count() > 0)
                                                                        {
                                                                            Node cn51 = new Node();
                                                                            cn51.Text = "AnswerID:" + qo.AnswerID;
                                                                            cn51.Tag = "AnswerID";
                                                                            cn51.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn51.Style.TextColor = Color.Gray;
                                                                            cn51.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn51);
                                                                        }




                                                                        DataRow[] cn52s = dt.Select("message = 'source沒有target有_q_o_OptionCode'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.OptionCode + "'");

                                                                        if (cn52s != null && cn52s.Count() > 0)
                                                                        {
                                                                            Node cn52 = new Node();
                                                                            cn52.Text = "OptionCode:" + qo.OptionCode;
                                                                            cn52.Tag = "OptionCode";
                                                                            cn52.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn52.Style.TextColor = Color.Gray;
                                                                            cn52.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn52);
                                                                        }


                                                                        DataRow[] cn53s = dt.Select("message = 'source沒有target有_q_o_OptionText'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.OptionText + "'");

                                                                        if (cn53s != null && cn53s.Count() > 0)
                                                                        {
                                                                            Node cn53 = new Node();
                                                                            cn53.Text = "OptionText:" + qo.OptionText;
                                                                            cn53.Tag = "OptionText";
                                                                            cn53.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn53.Style.TextColor = Color.Gray;
                                                                            cn53.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn53);
                                                                        }


                                                                        DataRow[] cn54s = dt.Select("message = 'source沒有target有_q_o_AnswerValue'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerValue + "'");

                                                                        if (cn54s != null && cn54s.Count() > 0)
                                                                        {
                                                                            Node cn54 = new Node();
                                                                            cn54.Text = "AnswerValue:" + qo.AnswerValue;
                                                                            cn54.Tag = "AnswerValue";
                                                                            cn54.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn54.Style.TextColor = Color.Gray;
                                                                            cn54.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn54);
                                                                        }


                                                                        DataRow[] cn55s = dt.Select("message = 'source沒有target有_q_o_AnswerMatrix'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "'  AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerMatrix + "'");

                                                                        if (cn55s != null && cn55s.Count() > 0)
                                                                        {
                                                                            Node cn55 = new Node();
                                                                            cn55.Text = "AnswerMatrix:" + qo.AnswerMatrix;
                                                                            cn55.Tag = "AnswerMatrix";
                                                                            cn55.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn55.Style.TextColor = Color.Gray;
                                                                            cn55.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn55);
                                                                        }


                                                                        DataRow[] cn56s = dt.Select("message = 'source沒有target有_q_o_AnswerChecked'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerChecked + "'");

                                                                        if (cn56s != null && cn56s.Count() > 0)
                                                                        {
                                                                            Node cn56 = new Node();
                                                                            cn56.Text = "AnswerChecked:" + qo.AnswerChecked;
                                                                            cn56.Tag = "AnswerChecked";
                                                                            cn56.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn56.Style.TextColor = Color.Gray;
                                                                            cn56.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn56);
                                                                        }


                                                                        DataRow[] cn57s = dt.Select("message = 'source沒有target有_q_o_AnswerComplete'  AND qt_id = '" + qt_id + "' AND qo_id = '" + qo_id + "' AND key = '" + subjInfo.Subject + "_" + qg.Group + "_" + qq.Query + "_" + qo.AnswerComplete + "'");

                                                                        if (cn57s != null && cn57s.Count() > 0)
                                                                        {
                                                                            Node cn57 = new Node();
                                                                            cn57.Text = "AnswerComplete:" + qo.AnswerComplete;
                                                                            cn57.Tag = "AnswerComplete";
                                                                            cn57.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            cn57.Style.TextColor = Color.Gray;
                                                                            cn57.Expand();
                                                                            checkExpendn5 = true;
                                                                            n5.Nodes.Add(cn57);
                                                                        }

                                                                        if (checkExpendn5)
                                                                        {
                                                                            n5.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                            n5.Style.TextColor = Color.Gray;
                                                                            checkAddN4 = true;
                                                                            n4.Nodes.Add(n5);                                                                           
                                                                            n5.Expand();
                                                                            n4.Expand();
                                                                            n3.Expand();
                                                                            n2.Expand();
                                                                            n1.Expand();
                                                                        }
                                                                    }

                                                                    if (checkExpendn4 == false && checkAddN4 == true)
                                                                    {
                                                                        n3.Nodes.Add(n4);
                                                                        n4.Style = new DevComponents.DotNetBar.ElementStyle();
                                                                        n4.Style.TextColor = Color.Gray;
                                                                        n4.Expand();
                                                                        n3.Expand();
                                                                        n2.Expand();
                                                                        n1.Expand();
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
                }
            }
        }

        private void advTree1_Click(object sender, EventArgs e)
        {

        }
    }

}

