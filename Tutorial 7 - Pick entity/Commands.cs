// Written by Felix Zhu @ CMS Surveyors

/********************************************************************************************************************************************
     In this tutorial, we will learn how to let user pick a Line and print its length in Command line.
     We will learn how to let user pick multiple entities (user selection) in next tutoiral.
     You don't need to write any code in this tutorial.
*********************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace Tutorial_7
{
    public class Commands
    {
        [CommandMethod("Tut7")]
        public void Tutorial_7_Pick_entity()
        {
            CivilDocument doc = CivilApplication.ActiveDocument;
            Document MdiActdoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = MdiActdoc.Editor;
            Database currentDB = MdiActdoc.Database;

            //since line (an entity) is picked by a user, it is a kind of "user input".
            //we need to use PromptEntityOptions and PromptEntityResult 

            PromptEntityOptions peo = new PromptEntityOptions("\nPlease pick a line:");
            peo.SetRejectMessage("\nOnly line can be picked.");                //for PromptEntityOptions, you have to set reject message. It is shown when user pick entites not allowed.
            peo.AddAllowedClass(typeof(Line), true);                          //Add Line into allowed class


            PromptEntityResult per = editor.GetEntity(peo);                  //Get entity result


            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction())
            {
                //We don't need to access modelSpace because nothing in modelSpace will be changed. ModelSpace is needed when you want to create a new Entity.

                if (per.Status == PromptStatus.OK)                               //check per.Status in case user pressing ESC
                {
                    try
                    {
                        ObjectId objId = per.ObjectId;
                        Line myline = trans.GetObject(objId, OpenMode.ForRead) as Line;   //This is how we access an object from ObjectId.
                                                                                          //Openmode.ForRead because we won't change anything, just need to read the length of Line.

                                                                                          //Think: We get object and transfer its type to Line because we know only Line can be picked.
                                                                                          //What if we add 3D Polyline into allowed class as well? You will find the answer in next tutorial.

                        editor.WriteMessage("\nThe length of line is " + myline.Length);

                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)                          //if something goes wrong and throw an exception, we will catch it.
                    {
                        editor.WriteMessage("\nError: " + ex.Message);
                    }

                }
                trans.Commit();
            }
        }
    }
}

//Build, load and run command "Tut7" in Civil3D.