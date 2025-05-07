using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;

namespace Shared.Services
{
    public class PrintStudyService : IPrintStudyService
    {
        private IRegularExpressionService _regularExpressionService;

        public PrintStudyService(IRegularExpressionService regularExpressionService)
        {
            _regularExpressionService = regularExpressionService;
        }

        public string ProjectName { get; set; }
        public string PrintStudySetFpath { get; set; }

        public void SetProjectName(string projectName)
        {
            ProjectName = projectName;
        }

        public void SetPrintStudySetFpath(string path)
        {
            PrintStudySetFpath = path;
        }
        public (List<Question> questions, List<Response> responses) GetQuestionResponseList(string fpath)
        {
            using (var sr = new StreamReader(fpath))
            {
                fpath = sr.ReadToEnd();
                List<Question> allQList = new List<Question>();
                List<Response> allRespList = new List<Response>();
                Dictionary<int, string> rowList = new Dictionary<int, string>();
                Dictionary<int, string> columnList = new Dictionary<int, string>();
                Dictionary<int, string> ResponseList = new Dictionary<int, string>();
                int stopPoint = 0;
                int counter = -1;

                string[] tempList;
                string[] tempVal;
                var responseText = string.Empty;

                var re = new Regex(@"^(Question Name: )[a-zA-Z0-9]*\r\n===========================\r\n(.*\r\n?)*?(===========================)\r\n", RegexOptions.Multiline);
                var arrQuestions = re.Matches(fpath);

                short questionOrder = 0;
                if (arrQuestions.Count > 0)
                {
                    #region Sorular

                    foreach (var objQuestion in arrQuestions)
                    {
                        try
                        {
                            var questionText = string.Empty;
                            var header1Text = string.Empty;
                            var header2Text = string.Empty;
                            var cornerLabel = string.Empty;
                            var gridHeader = string.Empty;
                            var questionDirection = string.Empty;
                            responseText = string.Empty;

                            re = new Regex(@"(?<=Question Name: )[a-zA-Z0-9]*(?=\r\n)");
                            var questionName = re.Match(objQuestion.ToString()).ToString();

                            re = new Regex(@"(?<============================\r\nType: ).*(?=\r\n)");
                            var questionType = re.Match(objQuestion.ToString()).ToString();

                            switch (questionType)
                            {
                                case "Constant Sum":
                                case "Numeric":
                                case "Open-end (multiple lines)":
                                case "Open-end (single line)":
                                case "Ranking (Numeric)":
                                case "Ranking (Combo Box)":
                                case "Select (Radio Button)":
                                case "Select (Check Box)":
                                case "Select (Combo Box)":

                                    #region Grid dışındaki soru tipleri

                                    header1Text = _regularExpressionService.rexp_h1(objQuestion.ToString());
                                    header2Text = _regularExpressionService.rexp_h2(objQuestion.ToString());
                                    questionText = _regularExpressionService.rexp_q(objQuestion.ToString());
                                    responseText = _regularExpressionService.rexp_r(objQuestion.ToString());

                                    /* Dikkat: stopPoint değişkenine checkbox ve ranking tarzı, spss .sav dosyasında
                                    herbir seçeneği bir soru olarak kabul edilen tipler için ihtiyac duyulmustur. */

                                    stopPoint = 1;

                                    /*******************************************************
                                        Cevap Seçenekleri İşleniyor
                                    *******************************************************/
                                    if (!string.IsNullOrEmpty(responseText))
                                    {
                                        ResponseList.Clear();

                                        /************************************
                                            Cevap seçenekleri okunuyor
                                        ************************************/
                                        tempList = responseText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        foreach (var entry in tempList)
                                        {
                                            tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            ResponseList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                                        }

                                        if (questionType == "Ranking (Numeric)" || questionType == "Ranking (Combo Box)" || questionType == "Constant Sum")
                                        {
                                            stopPoint = ResponseList.Count;
                                        }

                                        /*******************************************************
                                            Cevap seçenekleri alınıyor genel listeye ekleniyor.
                                        *******************************************************/
                                        if (questionType != "Constant Sum")
                                        {
                                            for (int i = 1; i <= stopPoint; i++)
                                            {
                                                foreach (KeyValuePair<int, string> entry in ResponseList)
                                                {
                                                    var clsResponse = new Response();

                                                    clsResponse.QuestionName = questionName;
                                                    clsResponse.ParentQuestionName = questionName;

                                                    clsResponse.ResponseText = entry.Value;
                                                    clsResponse.ResponseValue = entry.Key;

                                                    if (questionType == "Select (Check Box)")
                                                    {
                                                        clsResponse.QuestionName = questionName + "_" + entry.Key.ToString();
                                                    }
                                                    else if (questionType == "Ranking (Numeric)" || questionType == "Ranking (Combo Box)")
                                                    {
                                                        clsResponse.QuestionName = questionName + "_" + i.ToString();
                                                        clsResponse.ParentQuestionName = questionName + "_" + i.ToString();

                                                        clsResponse.ResponseText = entry.Key.ToString();
                                                    }
                                                    allRespList.Add(clsResponse);
                                                }
                                            }
                                        }
                                    }

                                    /*******************************************************/
                                    for (int i = 1; i <= stopPoint; i++)
                                    {
                                        /*******************************************************
                                            Soruya ait detaylar class'a ekleniyor.
                                        *******************************************************/
                                        var clsQuestion = new Question();
                                        clsQuestion.QuestionName = questionName;
                                        clsQuestion.QuestionType = questionType;
                                        clsQuestion.Header1 = header1Text;
                                        clsQuestion.Header2 = header2Text;
                                        clsQuestion.QuestionText = questionText;
                                        if (questionType == "Select (Check Box)")
                                        {
                                            clsQuestion.IsMultiple = true;
                                        }

                                        if (questionType == "Ranking (Numeric)" || questionType == "Ranking (Combo Box)" || questionType == "Constant Sum")
                                        {
                                            clsQuestion.QuestionName = questionName + "_" + i.ToString();
                                            clsQuestion.QuestionText = questionText + " " + ResponseList[i];
                                        }
                                        clsQuestion.ReportText = clsQuestion.QuestionText;
                                        //clsQuestion.ReportText = clsQuestion.Header1 + " " + clsQuestion.Header2 + " " + clsQuestion.QuestionText;

                                        questionOrder += 1;
                                        clsQuestion.QuestionOrder = questionOrder;
                                        allQList.Add(clsQuestion);
                                    }
                                    break;

                                #endregion Grid dışındaki soru tipleri

                                case "Grid":

                                    #region Grid soru tipi

                                    questionDirection = _regularExpressionService.rexp_grd_d(objQuestion.ToString());
                                    header1Text = _regularExpressionService.rexp_grd_h1(objQuestion.ToString());
                                    header2Text = _regularExpressionService.rexp_grd_h2(objQuestion.ToString());
                                    cornerLabel = _regularExpressionService.rexp_grd_cr(objQuestion.ToString());
                                    gridHeader = _regularExpressionService.rexp_grd_head(objQuestion.ToString());

                                    rowList.Clear();
                                    columnList.Clear();
                                    ResponseList.Clear();
                                    counter = -1;

                                    /*******************************************************
                                        Grid içerisinde ne kadar cevap text'i blogu varsa
                                        hepsi bir array'a blok blok alınıyor.
                                    *******************************************************/
                                    var arrResponses = _regularExpressionService.rexp_grd_r(objQuestion.ToString());
                                    if (arrResponses.Count > 0)
                                    {
                                        /***********************
                                            Row List alıyor.
                                        ***********************/
                                        counter += 1;
                                        tempList = arrResponses[counter].ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        foreach (var entry in tempList)
                                        {
                                            tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            rowList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                                        }

                                        /**************************
                                            Columns List alıyor.
                                        **************************/
                                        counter += 1;
                                        tempList = arrResponses[counter].ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        foreach (var entry in tempList)
                                        {
                                            tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            columnList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                                        }
                                    }

                                    if (questionDirection == "Rows")
                                    {
                                        re = new Regex(@"(?<=\[Row\s[0-9]+]:\r\nType: ).*(?=\r\n)");
                                        ResponseList = columnList;
                                    }
                                    else
                                    {
                                        re = new Regex(@"(?<=\[Column\s[0-9]+]:\r\nType: ).*(?=\r\n)");
                                        ResponseList = rowList;
                                    }

                                    var arrGrid = re.Matches(objQuestion.ToString());
                                    int gridCounter = 0;
                                    foreach (var objGrid in arrGrid)
                                    {
                                        // Question Type değişiyor.
                                        questionType = objGrid.ToString();

                                        gridCounter += 1;

                                        if (questionType == "Select (Combo Box)" || questionType == "Ranking (Combo Box)")
                                        {
                                            counter += 1;
                                            for (int rNumber = 1; rNumber <= ResponseList.Count(); rNumber++)
                                            {
                                                /*******************************************************
                                                    Soruya ait detaylar class'a ekleniyor.
                                                *******************************************************/
                                                var clsQuestion = new Question();

                                                if (questionDirection == "Rows")
                                                {
                                                    clsQuestion.QuestionName = questionName + "_r" + gridCounter + "_c" + rNumber;
                                                    clsQuestion.QuestionText = rowList[rNumber] + " " + columnList[gridCounter];
                                                }
                                                else
                                                {
                                                    clsQuestion.QuestionName = questionName + "_r" + rNumber + "_c" + gridCounter;
                                                    clsQuestion.QuestionText = columnList[gridCounter] + " " + rowList[rNumber];
                                                }

                                                clsQuestion.QuestionType = questionType;
                                                clsQuestion.QuestionDirection = questionDirection;
                                                clsQuestion.Header1 = header1Text;
                                                clsQuestion.Header2 = header2Text;
                                                clsQuestion.GridHeader = gridHeader;
                                                clsQuestion.CornerLabel = cornerLabel;

                                                clsQuestion.ReportText = clsQuestion.QuestionText;
                                                //clsQuestion.ReportText = clsQuestion.Header1 + " " + clsQuestion.Header2 + " " + clsQuestion.GridHeader + " " + clsQuestion.CornerLabel + " " + clsQuestion.QuestionText;

                                                tempList = arrResponses[counter].ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                for (int sNumber = 0; sNumber < tempList.Count(); sNumber++)
                                                {
                                                    tempVal = tempList[sNumber].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                                                    var clsResponse = new Response();
                                                    clsResponse.QuestionName = clsQuestion.QuestionName;
                                                    clsResponse.ParentQuestionName = clsResponse.QuestionName;
                                                    clsResponse.ResponseValue = Convert.ToInt32(tempVal[0]);
                                                    clsResponse.ResponseText = tempVal.Count() > 1 ? tempVal[1] : string.Empty;
                                                    allRespList.Add(clsResponse);
                                                }

                                                questionOrder += 1;
                                                clsQuestion.QuestionOrder = questionOrder;
                                                allQList.Add(clsQuestion);
                                            }
                                        }
                                        else
                                        {
                                            stopPoint = 1;
                                            if (questionType != "Select (Radio Button)" && questionType != "Select (Check Box)")
                                            {
                                                stopPoint = ResponseList.Count;
                                            }

                                            /*******************************************************
                                                Cevap seçenekleri alınıyor genel listeye ekleniyor.
                                            *******************************************************/
                                            if (questionType == "Select (Check Box)" || questionType == "Ranking (Numeric)" || questionType == "Select (Radio Button)")
                                            {
                                                for (int i = 1; i <= stopPoint; i++)
                                                {
                                                    foreach (var entry in ResponseList)
                                                    {
                                                        var clsResponse = new Response();
                                                        if (questionType == "Select (Radio Button)")
                                                        {
                                                            clsResponse.QuestionName = string.Format(questionName + "_{0}" + gridCounter, questionDirection == "Rows" ? "r" : "c");
                                                            clsResponse.ParentQuestionName = clsResponse.QuestionName;
                                                        }
                                                        else if (questionType == "Select (Check Box)")
                                                        {
                                                            if (questionDirection == "Rows")
                                                                clsResponse.QuestionName = questionName + "_r" + gridCounter + "_c" + entry.Key.ToString();
                                                            else
                                                                clsResponse.QuestionName = questionName + "_r" + entry.Key.ToString() + "_c" + gridCounter;

                                                            clsResponse.ParentQuestionName = string.Format(questionName + "_{0}" + gridCounter, questionDirection == "Rows" ? "r" : "c");
                                                        }
                                                        else // Ranking (Numeric)
                                                        {
                                                            if (questionDirection == "Rows")
                                                                clsResponse.QuestionName = questionName + "_r" + gridCounter + "_c" + i.ToString();
                                                            else
                                                                clsResponse.QuestionName = questionName + "_r" + i.ToString() + "_c" + gridCounter;

                                                            clsResponse.ParentQuestionName = clsResponse.QuestionName;
                                                        }

                                                        clsResponse.ResponseValue = entry.Key;

                                                        if (questionType == "Ranking (Numeric)")
                                                            clsResponse.ResponseText = entry.Key.ToString();
                                                        else
                                                            clsResponse.ResponseText = entry.Value;

                                                        allRespList.Add(clsResponse);
                                                    }
                                                }
                                            }

                                            /*******************************************************
                                                Soruya ait detaylar class`a ekleniyor.
                                            *******************************************************/
                                            for (int i = 1; i <= stopPoint; i++)
                                            {
                                                var clsQuestion = new Question();

                                                clsQuestion.QuestionText = string.Format(questionDirection == "Rows" ? rowList[gridCounter] : columnList[gridCounter]);
                                                if (questionType == "Select (Check Box)" || questionType == "Select (Radio Button)")
                                                {
                                                    clsQuestion.QuestionName = string.Format(questionName + "_{0}" + gridCounter, questionDirection == "Rows" ? "r" : "c");
                                                    if (questionType == "Select (Check Box)")
                                                    {
                                                        clsQuestion.IsMultiple = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (questionDirection == "Rows")
                                                        clsQuestion.QuestionName = questionName + "_r" + gridCounter + "_c" + i.ToString();
                                                    else
                                                        clsQuestion.QuestionName = questionName + "_r" + i.ToString() + "_c" + gridCounter;

                                                    clsQuestion.QuestionText += " " + ResponseList[i];
                                                }

                                                clsQuestion.QuestionType = questionType;
                                                clsQuestion.QuestionDirection = questionDirection;

                                                clsQuestion.Header1 = header1Text;
                                                clsQuestion.Header2 = header2Text;
                                                clsQuestion.GridHeader = gridHeader;
                                                clsQuestion.CornerLabel = cornerLabel;

                                                clsQuestion.ReportText = clsQuestion.QuestionText;
                                                //clsQuestion.ReportText = clsQuestion.Header1 + " " + clsQuestion.Header2 + " " + clsQuestion.GridHeader + " " + clsQuestion.CornerLabel + " " + clsQuestion.QuestionText;

                                                questionOrder += 1;
                                                clsQuestion.QuestionOrder = questionOrder;
                                                allQList.Add(clsQuestion);
                                            }

                                            /*******************************************************/
                                        }
                                    }
                                    break;

                                #endregion Grid soru tipi

                                case "Semantic Differential":

                                    #region Semantic soru tipi

                                    header1Text = _regularExpressionService.rexp_s_h1(objQuestion.ToString());
                                    header2Text = _regularExpressionService.rexp_s_h2(objQuestion.ToString());

                                    rowList.Clear();
                                    columnList.Clear();
                                    ResponseList.Clear();
                                    counter = -1;
                                    /*******************************************************
                                        Semantic içerisinde ne kadar cevap text'i blogu varsa
                                        hepsi bir array'a blok blok alınıyor.
                                    *******************************************************/
                                    arrResponses = _regularExpressionService.rexp_s_r(objQuestion.ToString());
                                    if (arrResponses.Count > 0)
                                    {
                                        /****************************
                                            Left Side List alınıyor.
                                        ****************************/
                                        counter += 1;
                                        tempList = arrResponses[counter].ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        foreach (var entry in tempList)
                                        {
                                            tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            rowList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                                        }

                                        /*****************************
                                            Right Side List alınıyor.
                                        *****************************/
                                        counter += 1;
                                        tempList = arrResponses[counter].ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        foreach (var entry in tempList)
                                        {
                                            tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            columnList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                                        }

                                        if (arrResponses.Count > 2)
                                        {
                                            /****************************
                                              Anchor Text List alınıyor.
                                            ****************************/
                                            counter += 1;
                                            tempList = arrResponses[counter].ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            foreach (var entry in tempList)
                                            {
                                                tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                ResponseList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                                            }
                                        }

                                        /*******************************************************
                                            Cevap seçenekleri alınıyor genel listeye ekleniyor.
                                        *******************************************************/

                                        for (int k = 1; k <= rowList.Count(); k++)
                                        {
                                            foreach (var entry in ResponseList)
                                            {
                                                var clsResponse = new Response();
                                                clsResponse.QuestionName = questionName + "_" + k.ToString();
                                                clsResponse.ParentQuestionName = clsResponse.QuestionName;
                                                clsResponse.ResponseValue = entry.Key;
                                                clsResponse.ResponseText = entry.Value;
                                                allRespList.Add(clsResponse);
                                            }
                                        }
                                        /*******************************************************/

                                        for (int k = 1; k <= rowList.Count(); k++)
                                        {
                                            /*******************************************************
                                                Soruya ait detaylar class'a ekleniyor.
                                            *******************************************************/
                                            var clsQuestion = new Question();
                                            clsQuestion.QuestionName = questionName + "_" + k.ToString();
                                            clsQuestion.QuestionType = questionType;
                                            clsQuestion.Header1 = header1Text;
                                            clsQuestion.Header2 = header2Text;
                                            clsQuestion.QuestionText = rowList[k] + " " + columnList[k];
                                            clsQuestion.ReportText = clsQuestion.QuestionText;
                                            //clsQuestion.ReportText = clsQuestion.Header1 + " " + clsQuestion.Header2 + " " + clsQuestion.QuestionText;

                                            questionOrder += 1;
                                            clsQuestion.QuestionOrder = questionOrder;
                                            allQList.Add(clsQuestion);
                                        }
                                    }
                                    break;

                                #endregion Semantic soru tipi

                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }

                    #endregion Sorular

                    #region Loop var mi?

                    /********************************************
                        Anketin içerisinde Loop tanımlanmış mı?
                    ********************************************/
                    re = new Regex(@"^(Loop Name: )[a-zA-Z0-9]*\r\n===========================\r\n(.*\r\n?)*?(===========================)\r\n", RegexOptions.Multiline);
                    var arrLoops = re.Matches(fpath);

                    if (arrLoops.Count > 0)
                    {
                        /***************************************************
                            Loop var ise print study'nin başında
                            tanımlanan Question List'e ihtiyacımız olacak.
                        ****************************************************/

                        re = new Regex(@"(?<=\r\nQuestion List\r\n========================================\r\n)(.*\r\n)*(?=\=*\r\nData Field Usage)");
                        var tempStr = re.Match(fpath).ToString().Trim().Replace("\t", "");

                        re = new Regex(@"(?<=\r\n\r\n)[a-zA-Z0-9]*\s(?=\()", RegexOptions.Multiline);
                        var arrQNames = re.Matches(tempStr);

                        /****************************************************/

                        foreach (var objLoop in arrLoops)
                        {
                            ResponseList.Clear();

                            re = new Regex(@"(?<=Loop Name: )[a-zA-Z0-9]*(?=\r\n)");
                            var loopName = re.Match(objLoop.ToString()).ToString();

                            /***************************************************************************
                                Loop List seçenekleri okunuyor
                            ***************************************************************************/
                            responseText = _regularExpressionService.rexp_r(objLoop.ToString());

                            tempList = responseText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            foreach (var entry in tempList)
                            {
                                tempVal = entry.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                ResponseList.Add(Convert.ToInt32(tempVal[0]), tempVal.Count() > 1 ? tempVal[1] : string.Empty);
                            }

                            /***************************************************************************
                                Hangi sorular arasında loop tanımlandığı öğreniliyor.
                            ***************************************************************************/

                            //Aşağıdaki regex'in eşleşeceği Örnek: Page 21 to 22 (S4A to S4B)

                            var re_loop1 = new Regex(@"(?<=(Page\s))[\d]+\sto\s[\d]+\s\([A-Za-z0-9]+\sto\s[A-Za-z0-9]+\)(?=\r\n)");
                            var tempLoopStr = re_loop1.Match(objLoop.ToString()).ToString();

                            var re_loop2 = new Regex(@"(?<=\()[a-zA-Z0-9]+\sto\s[a-zA-Z0-9]+(?=\))");
                            tempLoopStr = re_loop2.Match(tempLoopStr).ToString();

                            var arrLoopColumns = tempLoopStr.Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                            /***************************************************************************/
                            bool found = false;
                            string keyName = string.Empty;
                            foreach (var item in arrQNames)
                            {
                                keyName = item.ToString().Trim();
                                if (keyName.Equals(arrLoopColumns[0]) || found == true)
                                {
                                    found = true;

                                    var refQlist = (from myItem in allQList
                                                    where myItem.QuestionName == keyName || myItem.QuestionName.StartsWith(keyName + "_")
                                                    select (Question)myItem.Clone()
                                                   ).ToList();

                                    var refRespList = (from myItem in allRespList
                                                       where myItem.QuestionName == keyName || myItem.QuestionName.StartsWith(keyName + "_")
                                                       select (Response)myItem.Clone()
                                                      ).ToList();

                                    for (int j = 1; j <= ResponseList.Count(); j++)
                                    {
                                        /***************************************************************************
                                            Loop kolonları belli olduktan sonra, QuestionName'in sonuna ".1" eklemek
                                            gerekiyor.
                                        ***************************************************************************/
                                        if (j == 1)
                                        {
                                            (
                                                from myItem in allQList
                                                where myItem.QuestionName == keyName || myItem.QuestionName.StartsWith(keyName + "_")
                                                select myItem
                                            ).ToList().ForEach(myItem => myItem.QuestionName += "." + j.ToString());

                                            (
                                                from myItem in allRespList
                                                where myItem.QuestionName == keyName || myItem.QuestionName.StartsWith(keyName + "_")
                                                select myItem
                                            ).ToList().ForEach(myItem => { myItem.QuestionName += "." + j.ToString(); myItem.ParentQuestionName += "." + j.ToString(); });
                                        }
                                        else
                                        {
                                            /***************************************************************************
                                                LoopList içerisindeki eleman sayısı kadar, Loop sorularını kopyalamak
                                                gerekiyor.
                                            ***************************************************************************/
                                            var templist1 = (from myItem in refQlist
                                                             select (Question)myItem.Clone()
                                                            ).ToList();
                                            templist1.ForEach(x => x.QuestionName += "." + j.ToString());
                                            allQList.AddRange(templist1);

                                            var templist2 = (from myItem in refRespList
                                                             select (Response)myItem.Clone()
                                                            ).ToList();
                                            templist2.ForEach(x => { x.QuestionName += "." + j.ToString(); x.ParentQuestionName += "." + j.ToString(); });
                                            allRespList.AddRange(templist2);
                                        }
                                    }
                                }

                                if (keyName.Equals(arrLoopColumns[1]))
                                {
                                    break;
                                }
                            }
                        }
                    }

                    #endregion Loop var mi?

                    return (allQList, allRespList);

                }
                sr.Close();
                sr.Dispose();
            }
            return (null, null);
        }

    }
}
