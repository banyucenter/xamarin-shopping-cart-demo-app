using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.Speech.Tts;
using ManateeShoppingCart.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DroidMethods))]
namespace ManateeShoppingCart.Droid
{

    public class DroidMethods : Java.Lang.Object, NativeMethods
    {
        public DroidMethods() { }

        public Task ShowDialog(ListsModel _item, string _description)
        {
            var tcs = new TaskCompletionSource<bool>();

            var builder = new AlertDialog.Builder(Forms.Context);
            var inflater = Forms.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            var dialogView = inflater.Inflate(Resource.Layout.DroidContentDialog, null);
            AlertDialog dialog = null;
            builder.SetView(dialogView);

            builder.SetTitle(_description);

            builder.SetCancelable(true);

            EditText txtNewName = ((EditText)dialogView.FindViewById(Resource.Id.newName));
            txtNewName.KeyPress += (sender, e) =>
            {
                if (e.KeyCode == Keycode.Enter)
                {
                    if (txtNewName.Text.Trim().Length > 0)
                        _item.Name = txtNewName.Text;

                    dialog.Dismiss();
                }
            };

            // Add change button
            builder.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
            {
                if (txtNewName.Text.Trim().Length > 0)
                    _item.Name = txtNewName.Text;

                tcs.SetResult(true);
            });


            // Add cancel button
            builder.SetNegativeButton(Android.Resource.String.Cancel, (sender, e) =>
            {
                tcs.SetResult(false);
            });

            dialog = builder.Create();

            dialog.DismissEvent += (sender, e) =>
            {
                //just in case the user pressed the back button or press outside dialog
                tcs.TrySetResult(false);
            };

            dialog.Show();

            return tcs.Task;
        }

        public Task ShowDialog(ItemModel _item, string _description)
        {
            var tcs = new TaskCompletionSource<bool>();

            var builder = new AlertDialog.Builder(Forms.Context);
            var inflater = Forms.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            var dialogView = inflater.Inflate(Resource.Layout.DroidContentDialog, null);
            AlertDialog dialog = null;
            builder.SetView(dialogView);

            builder.SetTitle(_description);

            builder.SetCancelable(true);

            EditText txtNewName = ((EditText)dialogView.FindViewById(Resource.Id.newName));
            txtNewName.KeyPress += (sender, e) =>
            {
                if (e.KeyCode == Keycode.Enter)
                {
                    if (txtNewName.Text.Trim().Length > 0)
                        _item.Name = txtNewName.Text;

                    dialog.Dismiss();
                }
            };

            // Add change button
            builder.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
            {
                if (txtNewName.Text.Trim().Length > 0)
                    _item.Name = txtNewName.Text;

                tcs.SetResult(true);
            });


            // Add cancel button
            builder.SetNegativeButton(Android.Resource.String.Cancel, (sender, e) =>
            {
                tcs.SetResult(false);
            });

            dialog = builder.Create();

            dialog.DismissEvent += (sender, e) =>
            {
                //just in case the user pressed the back button or press outside dialog
                tcs.TrySetResult(false);
            };

            dialog.Show();

            return tcs.Task;
        }

        public void SetStatusBar(string _backgroundHexColor)
        {
            //Coloring the status bar is only available on Lollipop and later
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                try
                {
                    ((Activity)Forms.Context).Window.SetStatusBarColor(Android.Graphics.Color.ParseColor(_backgroundHexColor));
                }
                catch { }
            }
        }
    }
}