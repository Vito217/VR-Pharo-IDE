﻿using System.IO;
using System.Collections.Generic;
using UnityEngine;
using LoggingModule;
using AsyncSerializer;
using System.Threading.Tasks;

namespace SaveAndLoad
{
    public class SaveAndLoadModule : MonoBehaviour
    {
        private static bool inEditor = Application.isEditor;
        static string sessionPath = Path.Combine(Application.persistentDataPath, "session.data");
        static string baseDataPath = Path.Combine(Application.streamingAssetsPath, "BaseData", "session.data");

        public static string transcriptContents = "";
        public static SystemData sysData = new SystemData();
        public static List<Browser> browsers = new List<Browser>();
        public static List<Playground> playgrounds = new List<Playground>();
        public static List<Inspector> inspectors = new List<Inspector>();
        public static List<Graph> graphs = new List<Graph>();
        public static List<Transcript> transcripts = new List<Transcript>();

        public static List<BrowserData> SerializeBrowsers()
        {
            List<BrowserData> browsersData = new List<BrowserData>();
            foreach (Browser browser in browsers)
            {
                Vector3 pos = browser.transform.position;
                Vector3 fwd = browser.transform.forward;

                string lastPackageName = "", lastClassName = "";
                BrowserPackage lastPackage = browser.package_list.last_selected as BrowserPackage;
                if(lastPackage != null)
                {
                    lastPackageName = lastPackage.name;
                    Transform lastClass = browser.class_list.Find(lastPackageName);
                    if(lastClass != null)
                        lastClassName = lastClass.gameObject.GetComponent<ClassWindow>().last_selected.name;
                }
                string lastSideName = browser.lastSelectedSide;

                browsersData.Add(new BrowserData(pos, fwd, lastClassName, lastPackageName, lastSideName));
            }
            return browsersData;
        }

        public static void DeserializeBrowsers(Session session)
        {
            List<BrowserData> browsersData = session.browsers;

            foreach (BrowserData bdata in browsersData)
            {
                Vector3 pos = new Vector3(bdata.position.x, 0f, bdata.position.z);
                Vector3 fwd = new Vector3(bdata.forward.x, bdata.forward.y, bdata.forward.z);
                Vector3 final_pos = new Vector3(bdata.position.x, 2.25f, bdata.position.z);

                Browser browser = Instantiator.Instance.Browser();
                browser.Initialize(pos, final_pos, fwd);
                Transform lsp = browser.package_list.transform.Find(bdata.lastSelectedPackage);
                if (lsp != null && bdata.lastSelectedPackage != "")
                {
                    lsp.gameObject.GetComponent<BrowserPackage>().click();
                    Transform lsc = browser.class_list.Find(bdata.lastSelectedPackage).Find(bdata.lastSelectedClass);
                    if (lsc != null && bdata.lastSelectedClass != "")
                    {
                        lsc.gameObject.GetComponent<BrowserClass>().click();
                    }
                }
                if (bdata.lastSelectedSide == "ClassSide")
                    browser.onSelectClassSide();
                else
                    browser.onSelectInstanceSide();

                browsers.Add(browser);
                InteractionLogger.Count("Browser");
            }
        }

        public static List<PlaygroundData> SerializePlaygrounds()
        {
            List<PlaygroundData> playgroundsData = new List<PlaygroundData>();
            foreach (Playground playground in playgrounds)
            {
                Vector3 pos = playground.transform.position;
                Vector3 fwd = playground.transform.forward;
                string sourceCode = playground.field.text;
                playgroundsData.Add(new PlaygroundData(pos, fwd, sourceCode));
            }
            return playgroundsData;
        }

        public static void DeserializePlaygrounds(Session session)
        {
            List<PlaygroundData> playgroundsData = session.playgrounds;

            foreach (PlaygroundData pdata in playgroundsData)
            {
                Vector3 pos = new Vector3(pdata.position.x, 0f, pdata.position.z);
                Vector3 fwd = new Vector3(pdata.forward.x, pdata.forward.y, pdata.forward.z);
                Vector3 final_pos = new Vector3(pdata.position.x, 2f, pdata.position.z);

                Playground playground = Instantiator.Instance.Playground();
                playground.Initialize(pos, final_pos, fwd);
                playground.field.text = pdata.sourceCode;
                playgrounds.Add(playground);
                InteractionLogger.Count("Playground");
            }
        }

        public static List<InspectorData> SerializeInspectors()
        {
            List<InspectorData> inspectorsData = new List<InspectorData>();
            foreach (Inspector inspector in inspectors)
            {
                Vector3 pos = inspector.transform.position;
                Vector3 fwd = inspector.transform.forward;
                string rows = inspector.data;
                inspectorsData.Add(new InspectorData(pos, fwd, rows));
            }
            return inspectorsData;
        }

        public static void DeserializeInspectors(Session session)
        {
            List<InspectorData> inspectorsData = session.inspectors;

            foreach (InspectorData idata in inspectorsData)
            {
                Vector3 pos = new Vector3(idata.position.x, 0f, idata.position.z);
                Vector3 fwd = new Vector3(idata.forward.x, idata.forward.y, idata.forward.z);
                Vector3 final_pos = new Vector3(idata.position.x, 2f, idata.position.z);

                Inspector inspector = Instantiator.Instance.Inspector();
                inspector.setContent(idata.rows);
                inspector.Initialize(pos, final_pos, fwd);

                inspectors.Add(inspector);

                InteractionLogger.Count("Inspector");
            }
        }

        public static List<SVGData> SerializeGraphs()
        {
            List<SVGData> graphsData = new List<SVGData>();
            foreach (Graph graph in graphs)
            {
                Vector3 pos = graph.transform.position;
                Vector3 fwd = graph.transform.forward;
                string raw_image = graph.raw_image;
                string type = graph.type;
                graphsData.Add(new SVGData(pos, fwd, raw_image, type));
            }
            return graphsData;
        }

        public static void DeserializeGraphs(Session session)
        {
            List<SVGData> graphsData = session.graphs;

            foreach (SVGData gdata in graphsData)
            {
                Vector3 pos = new Vector3(gdata.position.x, 0f, gdata.position.z);
                Vector3 fwd = new Vector3(gdata.forward.x, gdata.forward.y, gdata.forward.z);
                Vector3 final_pos = new Vector3(gdata.position.x, 2f, gdata.position.z);

                string rawImage = gdata.rawImage;
                string type = gdata.type;

                Graph graph = Instantiator.Instance.Graph();
                graph.setSprite(rawImage, type);
                graph.Initialize(pos, final_pos, fwd);
                graphs.Add(graph);

                InteractionLogger.Count("GraphObject");
            }
        }

        public static async Task Save()
        {
            if (!inEditor)
            {
                if (!Directory.Exists(Application.persistentDataPath))
                    Directory.CreateDirectory(Application.persistentDataPath);

                Session s = new Session(
                    sysData,
                    SerializeBrowsers(),
                    SerializePlaygrounds(),
                    SerializeInspectors(),
                    SerializeGraphs()
                );
                await AsynchronousSerializer.Serialize(sessionPath, s);
            }
        }

        public static async Task Load()
        {
            if (!inEditor)
            {
                if (!Directory.Exists(Application.persistentDataPath))
                    Directory.CreateDirectory(Application.persistentDataPath);

                if (File.Exists(sessionPath))
                {
                    Session session = await AsynchronousSerializer.Deserialize(sessionPath);
                    await sysData.LoadData(session.classesAndMethods.data);
                    DeserializeBrowsers(session);
                    DeserializePlaygrounds(session);
                    DeserializeInspectors(session);
                    DeserializeGraphs(session);
                }
            }
        }
    }
}