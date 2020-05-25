namespace PTR.Models
{
    public class ProjectTypeModel : ModelBaseVM
    {
        string colour;
        public string Colour
        {
            get { return colour; }
            set { SetField(ref colour, value); }
        }

        bool showsponsor = false;
        public bool ShowSponsor
        {
            get { return showsponsor; }
            set { SetField(ref showsponsor, value); }
        }

        string misclabel;
        public string MiscellaneousDataLabel
        {
            get { return misclabel; }
            set { SetField(ref misclabel, value); }
        }

        bool showunitcost = false;
        public bool ShowUnitCost
        {
            get { return showunitcost; }
            set { SetField(ref showunitcost, value); }
        }


        bool showpriority;
        public bool ShowPriority
        {
            get { return showpriority; }
            set { SetField(ref showpriority, value); }
        }


        bool productrequired;
        public bool ProductRequired
        {
            get { return productrequired; }
            set { SetField(ref productrequired, value); }
        }

        bool salesrequired;
        public bool SalesRequired
        {
            get { return salesrequired; }
            set { SetField(ref salesrequired, value); }
        }

        bool salesvolumerequired;
        public bool SalesVolumeRequired
        {
            get { return salesvolumerequired; }
            set { SetField(ref salesvolumerequired, value); }
        }


        bool gmrequired;
        public bool GMRequired
        {
            get { return gmrequired; }
            set { SetField(ref gmrequired, value); }
        }

        bool mpcrequired;
        public bool MPCRequired
        {
            get { return mpcrequired; }
            set { SetField(ref mpcrequired, value); }
        }

        bool probabilityrequired;
        public bool ProbabilityRequired
        {
            get { return probabilityrequired; }
            set { SetField(ref probabilityrequired, value); }
        }

        bool opportunitycatrequired;
        public bool OpportunityCatRequired
        {
            get { return opportunitycatrequired; }
            set { SetField(ref opportunitycatrequired, value); }
        }

        bool showkpm;
        public bool ShowKPM
        {
            get { return showkpm; }
            set { SetField(ref showkpm, value); }
        }

        bool showdifftech;
        public bool ShowDifferentiatedTech
        {
            get { return showdifftech; }
            set { SetField(ref showdifftech, value); }
        }

        bool unitpricerequired;
        public bool UnitPriceRequired
        {
            get { return unitpricerequired; }
            set { SetField(ref unitpricerequired, value); }
        }

    }
}
