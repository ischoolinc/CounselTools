using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    /// <summary>
    /// 檢查項目使用
    /// </summary>
    public class ItemChecker
    {
        Dictionary<string, ItemCheck> itemCheckDict = new Dictionary<string, ItemCheck>();

        public void SetSource(string key,string item)
        {
            if (!itemCheckDict.ContainsKey(key))
            {
                ItemCheck ic = new ItemCheck();
                ic.Source = item;
                itemCheckDict.Add(key, ic);
            }
            else
            {
                itemCheckDict[key].Source = item;
            }
        }

        public void SetTarget(string key,string item)
        {
            if (!itemCheckDict.ContainsKey(key))
            {
                ItemCheck ic = new ItemCheck();
                ic.Target = item;
                itemCheckDict.Add(key, ic);
            }
            else
            {
                itemCheckDict[key].Target = item;
            }
        }

        public void Check()
        {
            foreach(string key in itemCheckDict.Keys)
            {
                if (itemCheckDict[key].Source == itemCheckDict[key].Target)
                    itemCheckDict[key].isSame = true;

                if (itemCheckDict[key].Source != "" && itemCheckDict[key].Target == "")
                    itemCheckDict[key].isOnlySource = true;

                if (itemCheckDict[key].Source == "" && itemCheckDict[key].Target != "")
                    itemCheckDict[key].isOnlyTarget = true;

            }
        }

        public Dictionary<string,ItemCheck> GetItemCheckAll()
        {
            return itemCheckDict;
        }

        public ItemCheck GetItemCheck(string key)
        {
            if (itemCheckDict.ContainsKey(key))
            {
                return itemCheckDict[key];
            }
            else
                return null;
        }
    }
}
