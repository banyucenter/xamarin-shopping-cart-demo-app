using ManateeShoppingCart.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(IOSMethods))]
namespace ManateeShoppingCart.iOS
{
    public class IOSMethods : NativeMethods
    {
        public Task ShowDialog(ListsModel _item, string _description)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                UIAlertController alert = UIAlertController.Create("", _description, UIAlertControllerStyle.Alert);
                UITextField field = null;

                // Add and configure text field
                alert.AddTextField((textField) =>
                {
                    // Save the field
                    field = textField;

                    // Initialize field
                    field.Placeholder = "Add new name";
                    field.AutocorrectionType = UITextAutocorrectionType.No;
                    field.KeyboardType = UIKeyboardType.Default;
                    field.ReturnKeyType = UIReturnKeyType.Done;
                    field.ClearButtonMode = UITextFieldViewMode.WhileEditing;
                });

                // Add cancel button
                alert.AddAction(UIAlertAction.Create("CANCEL", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    tcs.SetResult(false);
                }));

                // Add ok button
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                {

                    if (field.Text.Trim().Length > 0)
                        _item.Name = field.Text.Trim();

                    tcs.SetResult(true);
                }));

                // Display the alert
                UIWindow window = UIApplication.SharedApplication.KeyWindow;
                UIViewController controller = window.RootViewController;

                controller.PresentViewController(alert, true, null);
            }
            catch
            {
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }

        public Task ShowDialog(ItemModel _item, string _description)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                UIAlertController alert = UIAlertController.Create("", _description, UIAlertControllerStyle.Alert);
                UITextField field = null;

                // Add and configure text field
                alert.AddTextField((textField) =>
                {
                    // Save the field
                    field = textField;

                    // Initialize field
                    field.Placeholder = "Add new name";
                    field.AutocorrectionType = UITextAutocorrectionType.No;
                    field.KeyboardType = UIKeyboardType.Default;
                    field.ReturnKeyType = UIReturnKeyType.Done;
                    field.ClearButtonMode = UITextFieldViewMode.WhileEditing;
                });

                // Add cancel button
                alert.AddAction(UIAlertAction.Create("CANCEL", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    tcs.SetResult(false);
                }));

                // Add ok button
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                {

                    if (field.Text.Trim().Length > 0)
                        _item.Name = field.Text.Trim();

                    tcs.SetResult(true);
                }));

                // Display the alert
                UIWindow window = UIApplication.SharedApplication.KeyWindow;
                UIViewController controller = window.RootViewController;

                controller.PresentViewController(alert, true, null);
            }
            catch
            {
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }

        public void SetStatusBar(string _backgroundHexColor)
        {
        }
    }
}