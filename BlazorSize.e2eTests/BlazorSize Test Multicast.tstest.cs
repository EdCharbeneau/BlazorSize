using Telerik.TestStudio.Translators.Common;
using Telerik.TestingFramework.Controls.TelerikUI.Blazor;
using Telerik.TestingFramework.Controls.KendoUI.Angular;
using Telerik.TestingFramework.Controls.KendoUI;
using Telerik.WebAii.Controls.Html;
using Telerik.WebAii.Controls.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ArtOfTest.Common.UnitTesting;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Controls.HtmlControls.HtmlAsserts;
using ArtOfTest.WebAii.Design;
using ArtOfTest.WebAii.Design.Execution;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.Silverlight;
using ArtOfTest.WebAii.Silverlight.UI;

namespace BlazorSize.e2eTests
{

    public class BlazorSize_Test_Consolidation : BaseWebAiiTest
    {
        #region [ Dynamic Pages Reference ]

        private Pages _pages;

        /// <summary>
        /// Gets the Pages object that has references
        /// to all the elements, frames or regions
        /// in this project.
        /// </summary>
        public Pages Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new Pages(Manager.Current);
                }
                return _pages;
            }
        }

        #endregion
        
        // Add your test methods here...
    
        [CodedStep(@"Open Desktop")]
        public void OpenWin()
        {
                        Actions.InvokeScript(@"openWin(""multicast"", 1920, 1080)");
            
        }
    
        [CodedStep(@"Resize browser to tablet")]
        public void Resize_Tablet()
        {
                        Manager.Browsers[0].Actions.InvokeScript(@"resizeWin(800,800)");
            
        }
    
        [CodedStep(@"Resize browser to small")]
        public void Resize_Mobile()
        {
                        Manager.Browsers[0].Actions.InvokeScript(@"resizeWin(560,800)");
            
        }
    }
}
