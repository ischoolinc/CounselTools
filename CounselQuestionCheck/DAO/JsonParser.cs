using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using System.Windows.Forms;

namespace CounselQuestionCheck.DAO
{
    public class JsonParser
    {
        /// <summary>
        /// 讀取傳入路徑的 JSON檔案並解析成 SubjectInfo List
        /// </summary>
        /// <returns></returns>
        public List<SubjectInfo> LoadJSonFileAndParse(string FilePath)
        {
            List<SubjectInfo> subjInfoList = new List<SubjectInfo>();
            string Jsontext = "";
            dynamic JSONData = null;
            try
            {
                Jsontext = File.ReadAllText(FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取檔案失敗," + ex.Message);
            }

            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                JSONData = js.Deserialize<dynamic>(Jsontext);
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取 JSON 失敗," + ex.Message);
            }

            // 解析 json
            try
            {
                foreach (var jSubj in JSONData)
                {
                    if (jSubj["Subject"] != null)
                    {
                        SubjectInfo subjInfo = new SubjectInfo();
                        subjInfo.Subject = jSubj["Subject"];
                        subjInfo.QuestionGroupList = new List<QuestionGroup>();

                        if (jSubj["QuestionGroup"] != null)
                        {
                            foreach (var qGroup in jSubj["QuestionGroup"])
                            {
                                if (qGroup.Count < 2)
                                    continue;

                                QuestionGroup qg = new QuestionGroup();
                                if (qGroup["Group"] != null)
                                {
                                    qg.Group = qGroup["Group"];
                                    qg.QuestionQueryList = new List<QuestionQuery>();
                                }

                                if (qGroup["QuestionQuery"] != null)
                                {
                                    foreach (var qQuery in qGroup["QuestionQuery"])
                                    {
                                        if (qQuery.Count < 2)
                                            continue;
                                        QuestionQuery qq = new QuestionQuery();
                                        if (qQuery["Query"] != null)
                                        {                                            
                                            qq.Query = qQuery["Query"];
                                            qq.QuestionTextList = new List<QuestionText>();
                                        }

                                        if (qQuery["QuestionText"] != null)
                                        {
                                            foreach (var qText in qQuery["QuestionText"])
                                            {
                                                QuestionText qt = new QuestionText();
                                                qt.QuestionOptionsList = new List<QuestionOptions>();

                                                // 初始值 ""
                                                qt.QuestionCode = "";
                                                qt.Type = "";
                                                qt.Require = "";
                                                qt.RequireLink = "";
                                                qt.Text = "";

                                                if (qText["QuestionCode"] != null)
                                                {
                                                    qt.QuestionCode = qText["QuestionCode"];
                                                }

                                                if (qText["Options"] != null)
                                                {
                                                    foreach (var qOption in qText["Options"])
                                                    {
                                                        QuestionOptions qo = new QuestionOptions();

                                                        qo.AnswerID = "";
                                                        qo.OptionCode = "";
                                                        qo.OptionText = "";
                                                        qo.AnswerValue = "";
                                                        qo.AnswerMatrix = "";
                                                        qo.AnswerChecked = "";
                                                        qo.AnswerComplete = "";

                                                        if (qOption["AnswerID"] != null) {
                                                            qo.AnswerID = qOption["AnswerID"]+"";
                                                        }
                                                        if (qOption["OptionCode"] != null) {
                                                            qo.OptionCode = qOption["OptionCode"];
                                                        }
                                                        if (qOption["OptionText"] != null) {
                                                            qo.OptionText = qOption["OptionText"];
                                                        }
                                                        if (qOption["AnswerValue"] != null) {
                                                            qo.AnswerValue = qOption["AnswerValue"];
                                                        }
                                                        if (qOption["AnswerMatrix"] != null) {
                                                            qo.AnswerMatrix = qOption["AnswerMatrix"]+"";
                                                        }
                                                        if (qOption["AnswerChecked"] != null) {
                                                            qo.AnswerChecked = qOption["AnswerChecked"]+"";
                                                        }
                                                        if (qOption["AnswerComplete"] != null) {
                                                            qo.AnswerComplete = qOption["AnswerComplete"]+"";
                                                        }
                                                        qt.QuestionOptionsList.Add(qo);
                                                    }
                                                }

                                                if (qText["Type"] != null)
                                                {
                                                    qt.Type = qText["Type"];
                                                }                                              

                                                if (qText["Require"] != null)
                                                {
                                                    qt.Require = qText["Require"]+"";
                                                }
                                                if (qText["RequireLink"] != null)
                                                {
                                                    qt.RequireLink = qText["RequireLink"];
                                                }
                                                if (qText["Text"] != null)
                                                {
                                                    qt.Text = qText["Text"];
                                                }
                                                qq.QuestionTextList.Add(qt);
                                            }                                           
                                        }
                                        qg.QuestionQueryList.Add(qq);
                                    }                                   
                                }
                                subjInfo.QuestionGroupList.Add(qg);
                            }
                        }

                        subjInfoList.Add(subjInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("JSON 解析失敗," + ex.Message);
            }

            return subjInfoList;
        }
    }
}
