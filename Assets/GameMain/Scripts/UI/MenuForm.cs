﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using Pixel;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Pixel
{
    public class MenuForm : UGuiForm
    {
        [SerializeField]
        private GameObject m_QuitButton = null;

        private ProcedureMenu m_ProcedureMenu = null;
        
       public void OnLevelSelect()
       {
           Debug.Log("OnLevelSelect");
       }

       public void OnOptionsClick()
       {
           Debug.Log("OnOptionsClick");
       }

       public void OnQuitClick()
       {
           Debug.Log("OnQuitClick");
           GameEntry.UI.OpenDialog(new DialogParams()
           {
               Mode = 2,
               Title = GameEntry.Localization.GetString("AskQuitGame.Title"),
               Message = GameEntry.Localization.GetString("AskQuitGame.Message"),
               OnClickConfirm = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
           });
          
       }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);

            m_ProcedureMenu = (ProcedureMenu)userData;
            if (m_ProcedureMenu == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }

            m_QuitButton.SetActive(Application.platform != RuntimePlatform.IPhonePlayer);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(bool isShutdown, object userData)
#else
        protected internal override void OnClose(bool isShutdown, object userData)
#endif
        {
            m_ProcedureMenu = null;

            base.OnClose(isShutdown, userData);
        }
    }
}
