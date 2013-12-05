open System.Windows.Forms

let form = new Form(Text = "First F# form")
let button = new Button(Text = "Click me to close!", Dock = DockStyle.Fill)

button.Click.Add(fun _ -> Application.Exit() |> ignore)
form.Controls.Add(button)
form.Show()