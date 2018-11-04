using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using TruePosition.Test.DataLayer;
using System.Diagnostics;

namespace TruePosition.Test.Custom.CSharp
{
    public static class CustomCommandEvents
    {
        // DESIGN NOTE:
        // Global values will be stored in a global value cache accessible to all system components...
        private static bool RMA = true;
        private static string ScannedCustomerESN = "TRULMU5207872AE";
        private static string FPGAType;
        private static int DownloadType = 2;
        private static bool UpperBoardPresent, DcardEsnFailure;
        private const int DWNLD_CUSTOM_REV = 2;

        [ResponseProcessing(ProcessingEvent.ElementProcessing)]
        public static void Element_Processing(object sender, ResponseElementProcessingArgs args)
        {
            Debug.Print("<Event=Element_Processing,\tRaw Element=" + args.RawElement + ">");
        }
        [ResponseProcessed(ProcessedEvent.ElementProcessed)]
        public static void Element_Processed(object sender, ResponseElementProcessedArgs args)
        {
            Debug.Print("<Event=Element_Processed,\tRaw Element=" + args.RawElement + ">");
        }
        // Command: CUSTESN
        [ResponseProcessing("1A", 5, 2, 1)]
        public static void CustomerESN_Processing(object sender, ExpectedResponseProcessingArgs args)
        {
            Debug.Print("<Event=CustomerESN_Processing,\tRaw Element=" + args.RawElement + ">");
        }
        [ResponseProcessed("1A", 5, 2, 1)]
        public static void CustomerESN_Processed(object sender, ExpectedResponseProcessedArgs args)
        {
            Debug.Print("<Event=CustomerESN_Processed,\tRaw Element=" + args.RawElement + ">");

            ExpectedResponse er = (ExpectedResponse)sender;
            if ((RMA) && (args.Value.GetString() != ScannedCustomerESN))
                throw new InvalidOperationException("ESN/barcode mismatch");
        }

        // Command: CP/DSP
        [ResponseProcessing("1A", 5, 7, 1)]
        public static void CPDSP_Processin(object sengder, ExpectedResponseProcessingArgs args)
        {
        }
        [ResponseProcessed("1A", 5, 7, 1)]
        public static void CPDSP_Processed(object sender, ExpectedResponseProcessedArgs args)
        {
            ExpectedResponse er = (ExpectedResponse)sender;
            if (er.Value.GetString().Contains("061600"))
                FPGAType = "O";  // O stands for Old CPDSP
            else if (er.Value.GetString().Contains("061641"))
                FPGAType = "E";  // E stands for E-CPDSP
            else if (er.Value.GetString().Contains("061644"))
                FPGAType = "N";  // N stands for NON-ECPDSP
        }

        // Command: GPS RCVR
        [ResponseProcessing("1A", 5, 3, 1)]
        public static void GPSRCVR_Processed(object sender, ExpectedResponseProcessingArgs args)
        {
        }
        [ResponseProcessed("1A", 5, 3, 1)]
        public static void GPSRCVR_Processed(object sender, ExpectedResponseProcessedArgs args)
        {
            ExpectedResponse er = (ExpectedResponse)sender;
            if (DownloadType != DWNLD_CUSTOM_REV)
                throw new InvalidOperationException("Bad GPS ESN");
        }

        // Command: BDC
        [ResponseProcessing("1A", 5, 5, 1)]
        public static void BDC_Processed(object sender, ExpectedResponseProcessingArgs args)
        {
        }
        [ResponseProcessed("1A", 5, 5, 1)]
        public static void BDC_Processed(object sender, ExpectedResponseProcessedArgs args)
        {
            ExpectedResponse er = (ExpectedResponse)sender;
            if (er.Value.GetString().Contains("NOT INSTALLED"))
                UpperBoardPresent = false;
            else
                UpperBoardPresent = true;
        }

        // Command: DCARD
        [ResponseProcessing("1A", 5, 8, 1)]
        public static void DCARD_Processed(object sender, ExpectedResponseProcessingArgs args)
        {
        }
        [ResponseProcessed("1A", 5, 8, 1)]
        public static void DCARD_Processed(object sender, ExpectedResponseProcessedArgs args)
        {
            ExpectedResponse er = (ExpectedResponse)sender;
            if (er.Value.GetString().Contains("DEBUG"))
                DcardEsnFailure = true;
        }
    }
}
