using System.ComponentModel;

namespace LivePrices;

public partial class SymbolSelectForm : Form
{
    [DefaultValue(false)]
    public bool OkClicked { get; set; }

    [DefaultValue(false)]
    public bool CancelClicked { get; set; }

    [DefaultValue("")]
    public string? Symbol1 { get; set; }

    [DefaultValue("")]
    public string? Symbol2 { get; set; }

    public SymbolSelectForm()
    {
        InitializeComponent();
    }

    public SymbolSelectForm(List<string> symbolsList) : this()
    {
        foreach (var symbol in symbolsList)
        {
            cbx_Symbol1.Items.Add(symbol);
            cbx_Symbol2.Items.Add(symbol);
        }
    }

    private void btn_Ok_Click(object sender, EventArgs e)
    {
        Symbol1 = (string)cbx_Symbol1.SelectedItem;
        Symbol2 = (string)cbx_Symbol2.SelectedItem;
        OkClicked = true;
        Close();
    }

    private void btn_Cancel_Click(object sender, EventArgs e)
    {
        CancelClicked = true;
        Close();
    }

    private void SymbolSelectForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!OkClicked)
        {
            CancelClicked = true;
            OkClicked = false;
        }
    }
}
