using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Content.PM;

namespace ManateeShoppingCart.Droid
{
    [Activity(Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
                                        {
                                            Task.Delay(5000); // Simulate a bit of startup work.
                                        });

            startupWork.ContinueWith(t =>
                                     {
                                         StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                                     }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}