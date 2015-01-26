﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomServerControls
{
    /// <summary>
    /// A <see cref="DecoratedButton"/> which, when clicked, executes a javascript function
    /// that transforms text wtitten in Bulgarian using Latin letters to a text written with
    /// Bulgarian letters.
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TransliterateButton runat=server></{0}:TransliterateButton>")]
    public class TransliterateButton : DecoratedButton
    {
        /// <summary>
        /// The key that identifies the transliterate javascript is to be registered.
        /// </summary>
        public string ClientScriptKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TextBox"/> which contents will be transliterated.
        /// <para>If this property is not <c>null</c>, the <c>TargetTextArea</c> property must be <c>null</c>.</para>
        /// </summary>
        public TextBox TargetTextBox { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Web.UI.HtmlControls.HtmlTextArea"/> which contents will be transliterated.
        /// <para>If this property is not <c>null</c>, the <c>TargetTextBox</c> property must be <c>null</c>.</para>
        /// </summary>
        public string TargetTextArea { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransliterateButton"/> class.
        /// </summary>
        public TransliterateButton()
            : base()
        {
            ClientScriptKey = "TransliterateBgLatinToBgCyrillic";
            base.Text = "Кирилица";
            base.ToolTip = "Транслитерира (преобразува) латиница в кирилица";
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="HtmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (base.Visible == true)
            {
                string targetControlID = null;

                if (TargetTextBox != null)
                {
                    targetControlID = TargetTextBox.ClientID;
                }
                if (TargetTextArea != null)
                {
                    if (!string.IsNullOrEmpty(targetControlID))
                    {
                        string errMsg = string.Format("Ambiquity: More than one target control is specified.");
                        throw new InvalidOperationException(errMsg);
                    }
                    targetControlID = TargetTextArea;
                }
                if (!string.IsNullOrEmpty(targetControlID))
                {
                    string jsFuncCall = string.Format("transliterate_contents_bg_lc(\"{0}\"); return false;",
                        targetControlID);
                    base.Attributes.Remove("onclick");
                    base.Attributes.Add("onclick", jsFuncCall);

                    RegisterJavascriptFunction(writer);
                }
                else
                {
                    // No target is set.
                    if (DesignMode == false)
                    {
                        throw new InvalidOperationException("No target specified.");
                    }
                }
            }

            base.RenderControl(writer);
        }

        /// <summary>
        /// Inserts the javascript function into the page, if not already inserted.
        /// </summary>
        private void RegisterJavascriptFunction(HtmlTextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (Page == null)
            {
                throw new InvalidOperationException("Page" + " is null.");
            }
            if (string.IsNullOrEmpty(ClientScriptKey) == true)
            {
                throw new InvalidOperationException("ClientScriptKey" + " is null or empty.");
            }

            Type pageType = Page.GetType();

            if (this.Page.ClientScript.IsClientScriptBlockRegistered(pageType, ClientScriptKey) == false)
            {
                string javascriptFunction = @"
// ""lc"" means from latin letters to cyrillic letters 
function transliterate_contents_bg_lc(textbox_id) {
    if (textbox_id) {
        var textbox = document.getElementById(textbox_id);
        if (textbox) {
            var txt = textbox.value;
            if(txt) {

                // Longest combinations first
                txt = txt.replace(/^Bulgaria/g, 'България');
                txt = txt.replace(/ Bulgaria/g, ' България');

                // щ, Щ
                txt = txt.replace(/sht/g, 'щ');
                txt = txt.replace(/sHt/g, 'щ');
                txt = txt.replace(/shT/g, 'щ');
                txt = txt.replace(/sHT/g, 'щ');
                txt = txt.replace(/Sht/g, 'Щ');
                txt = txt.replace(/SHt/g, 'Щ');
                txt = txt.replace(/SHT/g, 'Щ');
                txt = txt.replace(/ShT/g, 'Щ');

                // ш, Ш
                txt = txt.replace( /sh/g, 'ш');
                txt = txt.replace( /sH/g, 'ш');
                txt = txt.replace( /Sh/g, 'Ш');
                txt = txt.replace( /SH/g, 'Ш');
                // ж, Ж
                txt = txt.replace( /zh/g, 'ж');
                txt = txt.replace( /zH/g, 'ж');
                txt = txt.replace( /Zh/g, 'Ж');
                txt = txt.replace( /ZH/g, 'Ж');
                // ю, Ю
                txt = txt.replace( /yu/g, 'ю');
                txt = txt.replace( /yU/g, 'ю');
                txt = txt.replace( /Yu/g, 'Ю');
                txt = txt.replace( /YU/g, 'Ю');
                // я, Я
                txt = txt.replace( /ya/g, 'я');
                txt = txt.replace( /yA/g, 'я');
                txt = txt.replace( /Ya/g, 'Я');
                txt = txt.replace( /YA/g, 'Я');
                // ч, Ч
                txt = txt.replace( /ch/g, 'ч');
                txt = txt.replace( /cH/g, 'ч');
                txt = txt.replace( /Ch/g, 'Ч');
                txt = txt.replace( /CH/g, 'Ч');
                // ц, Ц
                txt = txt.replace( /ts/g, 'ц');
                txt = txt.replace( /tS/g, 'ц');
                txt = txt.replace( /Ts/g, 'Ц');
                txt = txt.replace( /TS/g, 'Ц');

                // ия, ИЯ at the end of the word
                txt = txt.replace(/ia /g, 'ия ');
                txt = txt.replace(/iA /g, 'иЯ ');
                txt = txt.replace(/Ia /g, 'Ия ');
                txt = txt.replace(/IA /g, 'ИЯ ');
                txt = txt.replace(/ia./g, 'ия.');
                txt = txt.replace(/iA./g, 'иЯ.');
                txt = txt.replace(/Ia./g, 'Ия.');
                txt = txt.replace(/IA./g, 'ИЯ.');
                txt = txt.replace(/ia,/g, 'ия,');
                txt = txt.replace(/iA,/g, 'иЯ,');
                txt = txt.replace(/Ia,/g, 'Ия,');
                txt = txt.replace(/IA,/g, 'ИЯ,');
                // ия, ИЯ at the end of the text
                txt = txt.replace(/ia$/g, 'ия');
                txt = txt.replace(/iA$/g, 'иЯ');
                txt = txt.replace(/Ia$/g, 'Ия');
                txt = txt.replace(/IA$/g, 'ИЯ');

                // These are 3 symbols long, but have lower precedence than the 2-symbol combinations above
                // ай, Ай
                txt = txt.replace( /ay/g, 'ай');
                txt = txt.replace( /aY/g, 'аЙ');
                txt = txt.replace( /AY/g, 'АЙ');
                txt = txt.replace( /Ay/g, 'Ай');
                // ей, Ей
                txt = txt.replace( /ey/g, 'ей');
                txt = txt.replace( /eY/g, 'еЙ');
                txt = txt.replace( /EY/g, 'ЕЙ');
                txt = txt.replace( /Ey/g, 'Ей');
                // ий, Ий
                txt = txt.replace( /iy/g, 'ий');
                txt = txt.replace( /iY/g, 'иЙ');
                txt = txt.replace( /IY/g, 'ИЙ');
                txt = txt.replace( /Iy/g, 'Ий');
                // ой, Ой
                txt = txt.replace( /oy/g, 'ой');
                txt = txt.replace( /oY/g, 'оЙ');
                txt = txt.replace( /OY/g, 'ОЙ');
                txt = txt.replace( /Oy/g, 'Ой');
                // уй, Уй
                txt = txt.replace( /uy/g, 'уй');
                txt = txt.replace( /uY/g, 'уЙ');
                txt = txt.replace( /UY/g, 'УЙ');
                txt = txt.replace( /Uy/g, 'Уй');
                // юй, Юй, ЮЙ
                txt = txt.replace(/yuy/g, 'юй');
                txt = txt.replace(/yUy/g, 'юй');
                txt = txt.replace(/Yuy/g, 'Юй');
                txt = txt.replace(/YUy/g, 'Юй');
                txt = txt.replace(/yuY/g, 'юЙ');
                txt = txt.replace(/yUY/g, 'юЙ');
                txt = txt.replace(/YuY/g, 'ЮЙ');
                txt = txt.replace(/YUY/g, 'ЮЙ');
                // яй, Яй, ЯЙ
                txt = txt.replace(/yay/g, 'яй');
                txt = txt.replace(/yAy/g, 'яй');
                txt = txt.replace(/Yay/g, 'Яй');
                txt = txt.replace(/YAy/g, 'Яй');
                txt = txt.replace(/yaY/g, 'яЙ');
                txt = txt.replace(/yAY/g, 'яЙ');
                txt = txt.replace(/YaY/g, 'ЯЙ');
                txt = txt.replace(/YAY/g, 'ЯЙ');

                // Single symbols
                // а, А
                txt = txt.replace(  /a/g, 'а');
                txt = txt.replace(  /A/g, 'А');
                // б, Б
                txt = txt.replace(  /b/g, 'б');
                txt = txt.replace(  /B/g, 'Б');
                // в, В
                txt = txt.replace(  /v/g, 'в');
                txt = txt.replace(  /V/g, 'В');
                txt = txt.replace(  /w/g, 'в');
                txt = txt.replace(  /W/g, 'В');
                // г, Г
                txt = txt.replace(  /g/g, 'г');
                txt = txt.replace(  /G/g, 'Г');
                // д, Д
                txt = txt.replace(  /d/g, 'д');
                txt = txt.replace(  /D/g, 'Д');
                // е, Е
                txt = txt.replace(  /e/g, 'е');
                txt = txt.replace(  /E/g, 'Е');
                // ж, Ж (as an exception. It is expected that 'ж' is written as 'zh')
                txt = txt.replace(  /j/g, 'ж');
                txt = txt.replace(  /J/g, 'Ж');
                // з, З
                txt = txt.replace(  /z/g, 'з');
                txt = txt.replace(  /Z/g, 'З');
                // и, И
                txt = txt.replace(  /i/g, 'и');
                txt = txt.replace(  /I/g, 'И');
                // й, Й at the beginning of a word
                txt = txt.replace(  /^y/g, 'й');
                txt = txt.replace(  /^Y/g, 'Й');
                txt = txt.replace(  / y/g, ' й');
                txt = txt.replace(  / Y/g, ' Й');
                // к, К
                txt = txt.replace(  /k/g, 'к');
                txt = txt.replace(  /K/g, 'К');
                // л, Л
                txt = txt.replace(  /l/g, 'л');
                txt = txt.replace(  /L/g, 'Л');
                // м, М
                txt = txt.replace(  /m/g, 'м');
                txt = txt.replace(  /M/g, 'М');
                // н, Н
                txt = txt.replace(  /n/g, 'н');
                txt = txt.replace(  /N/g, 'Н');
                // о, О
                txt = txt.replace(  /o/g, 'о');
                txt = txt.replace(  /O/g, 'О');
                // п, П
                txt = txt.replace(  /p/g, 'п');
                txt = txt.replace(  /P/g, 'П');
                // р, Р
                txt = txt.replace(  /r/g, 'р');
                txt = txt.replace(  /R/g, 'Р');
                // с, С
                txt = txt.replace(  /s/g, 'с');
                txt = txt.replace(  /S/g, 'С');
                // т, Т
                txt = txt.replace(  /t/g, 'т');
                txt = txt.replace(  /T/g, 'Т');
                // у, У
                txt = txt.replace(  /u/g, 'у');
                txt = txt.replace(  /U/g, 'У');
                // Ф, Ф
                txt = txt.replace(  /f/g, 'ф');
                txt = txt.replace(  /F/g, 'Ф');
                // х, Х
                txt = txt.replace(  /h/g, 'х');
                txt = txt.replace(  /H/g, 'Х');
                // ц, Ц (as an exception. It is expected that 'ц' is written as 'ts')
                txt = txt.replace(  /c/g, 'ц');
                txt = txt.replace(  /C/g, 'Ц');
                // Cannot derive 'ъ' from Latin symbol
                //txt = txt.replace(  /?/g, 'ъ');

                // ь, Ь
                txt = txt.replace(  /y/g, 'ь');
                txt = txt.replace(  /Y/g, 'Ь');
                // я, Я (as an exception. It is expected that 'я' is written as 'ya')
                txt = txt.replace(  /q/g, 'я');
                txt = txt.replace(  /Q/g, 'Я');

                // ...

              //if(confirm(txt + '\r\n\r\nЗа да имате буквата \'ъ\', пишете на кирилица :)\r\nПотвърждавате ли кирилизирането?')) {
                    textbox.value = txt;
              //}
            }
        }
        else {
            alert(""No element with id='"" + textbox_id + ""'."");
        }
    }
    else {
        alert(""textbox_id is missing."");
    }
}
";
                string wholeJavascript = javascriptFunction;

                this.Page.ClientScript.RegisterClientScriptBlock(pageType, ClientScriptKey, wholeJavascript, true);
                if (this.Page.ClientScript.IsClientScriptBlockRegistered(pageType, ClientScriptKey) == true)
                {
                    writer.WriteBeginTag("script");
                    writer.WriteAttribute("type", "text/javascript");
                    writer.WriteAttribute("language", "javascript");
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Write(wholeJavascript);
                    writer.WriteEndTag("script");
                }
                else
                {
                    throw new InvalidOperationException(ClientScriptKey + " client script could not be registered.");
                }
            }
        }

    }
}
