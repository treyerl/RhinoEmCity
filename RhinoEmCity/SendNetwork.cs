using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Luci;
using System.Text;
using System.Drawing;

namespace RhinoEmCity
{
    public class SendNetwork : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SendNetwork() : base("Send Street Network", "Network", "A component to connect to EmCity via Luci", "EmCity", "Luci")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("LUCI host address", "host", "The IP address of LUCI server which offers EmCity service", GH_ParamAccess.item, "localhost");
            pManager.AddIntegerParameter("LUCI host port", "port", "The port to which data has to be sent on LUCI server", GH_ParamAccess.item, 7654);
            pManager.AddTextParameter("Street Points", "street_points", "Float Numbers; one number per line", GH_ParamAccess.list);
            pManager.AddTextParameter("Street Indices", "street_indices", "Integer Numbers; one number per line", GH_ParamAccess.list);
            pManager.AddTextParameter("Parcel Points", "parcel_points", "Float Numbers; one number per line", GH_ParamAccess.list);
            pManager.AddTextParameter("Parcel Indices", "parcel_indices", "Integer Numbers; one number per line", GH_ParamAccess.list);
        }

        ///// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string host = "";
            int port = 0;
            List<string> street_points = new List<string>();
            List<string> street_indices = new List<string>();
            List<string> parcel_points = new List<string>();
            List<string> parcel_indices = new List<string>();

            if ((!DA.GetData<string>(0, ref host)))
                return;
            if ((!DA.GetData<int>(1, ref port)))
                return;
            if ((!DA.GetDataList<string>(2, street_points)))
                return;
            if ((!DA.GetDataList<string>(3, street_indices)))
                return;
            if ((!DA.GetDataList<string>(4, parcel_points)))
                return;
            if ((!DA.GetDataList<string>(5, parcel_indices)))
                return;

            Client cl = new Client();
            cl.connect(host, port);
            Attachment a_street_points = new ArrayAttachment("txt", Encoding.UTF8.GetBytes(string.Join("\n", street_points)));
            Attachment a_street_indices = new ArrayAttachment("txt", Encoding.UTF8.GetBytes(string.Join("\n", street_indices)));
            Attachment a_parcel_points = new ArrayAttachment("txt", Encoding.UTF8.GetBytes(string.Join("\n", parcel_points)));
            Attachment a_parcel_indices = new ArrayAttachment("txt", Encoding.UTF8.GetBytes(string.Join("\n", parcel_indices)));
            Message message = cl.sendMessageAndReceiveResults(new Message(new
            {
                run = "EmCity",
                network = new
                {
                    streets = new { points = a_street_points, indices = a_street_indices },
                    parcels = new { points = a_parcel_points, indices = a_parcel_indices },
                }
            }));
            var list = new List<string> { "hello", "world" };
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return (Bitmap)Bitmap.FromFile("../network.bmp", true);
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{6ac1fb0c-4f4c-4220-9f9c-d3721681d92e}"); }
        }
    }
}
