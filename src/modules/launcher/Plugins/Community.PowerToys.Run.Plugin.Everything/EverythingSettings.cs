// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.PowerToys.Run.Plugin.Everything
{
    internal class EverythingSettings
    {
        public List<MacroSettings> Macros { get; } = new List<MacroSettings>()
        {
            new MacroSettings() { Prefix = "doc", FileExtensions = "c,chm,cpp,doc,dot,h,htm,html,mht,mhtml,nfo,pdf,pps,ppt,rtf,txt,vsd,wpd,wps,wri,xls,xml,txt,docx,htm,html,pdf,c,cpp,h,xls,odp,odt,ods,pptx,xlsx,csv,docx,ppsx,java,hpp,ini,dotx,xlsb" },
            new MacroSettings() { Prefix = "audio", FileExtensions = "aac,ac3,aif,aifc,aiff,au,cda,dts,fla,flac,gym,it,m1a,m2a,m4a,midi,mka,mod,mp2,mp3,mpa,ogg,ra,spc,rmi,snd,umx,vgm,vgz,voc,wav,wma,xm" },
            new MacroSettings() { Prefix = "zip", FileExtensions = "ace,arj,bz2,cab,gz,gzip,r00,r01,r02,r03,r04,r05,r06,r07,r08,r09,r10,r11,r12,r13,r14,r15,r16,r17,r18,r19,r20,r21,r22,r23,r24,r25,r26,r27,r28,r29,rar,tar,7z，zip" },
            new MacroSettings() { Prefix = "pic", FileExtensions = "ani,bmp,gif,ico,jpe,jpeg,jpg,pcx,png,psd,tga,tif,tiff,wmf,wbmp,icl,jp2,mpng,raw,nef,wdp,hdp" },
            new MacroSettings() { Prefix = "video", FileExtensions = "3g2,3gp,3gp2,3gpp,amr,asf,avi,bik,d2v,dat,divx,drc,dsa,dsm,dss,dsv,flc,fli,flic,flv,ifo,ivf,m1v,m2v,m4b,m4p,m4v,mkv,mp2v,mp4,mpe,mpeg,mpg,mpv2,mov,ogm,pss,pva,qt,ram,ratdvd,rm,rmm,roq,rpm,smk,swf,tp,tpr,ts,vob,vp6,wm,wmp,wmv,rmvb" },
            new MacroSettings() { Prefix = "web", FileExtensions = "html,htm,mht,php,css,sql,js" },
            new MacroSettings() { Prefix = "exe", FileExtensions = "exe,cmd,msi,msix" },
        };

        public int MaxSearchCount { get; set; } = 30;

        public bool UseLocationAsWorkingDir { get; set; }
    }
}
