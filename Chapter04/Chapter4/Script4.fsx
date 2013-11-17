#r "System.Windows.Forms.DataVisualization.dll"

// The form
open System
open System.Net
open System.Windows.Forms
open System.Drawing

let form = new Form(Visible = true, Text = "Data grid #1",
                    TopMost = true, Size = Drawing.Size(600,600))

let textBox = 
    new RichTextBox(Dock = DockStyle.Fill, Text = "F# Programming is Fun!",
                    Font = new Font("Lucida Console",16.0f,FontStyle.Bold),
                    ForeColor = Color.DarkBlue)

let show x = 
   textBox.Text <- sprintf "%30A" x


form.Controls.Add textBox

show (1,2)
show [ 0 .. 100 ]
show [ 0.0 .. 2.0 .. 100.0 ]

(1,2,3) |> show

[ 0 .. 99 ] |> show

[ for i in 0 .. 99 -> (i, i*i) ] |> show
