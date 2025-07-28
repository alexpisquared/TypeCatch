using System.Windows;
using System.Windows.Input;

namespace TypingWpf
{
  public partial class MainWindow // : WindowBase
  {
    public MainWindow() : base()
    {
      InitializeComponent();

      _ignoreEscape = true;

      //VersioInfo.Text = $"{"VerHelper.CurVerStr(.Net5)"}\n{A0DbMdl.GetA0DbMdl.ServerDatabase()}";

      //Topmost = Debugger.IsAttached;
    }

    /*
    <Popup x:Name="DraggablePopup" PlacementTarget="{Binding ElementName=vizKey2}" Placement="Left" HorizontalOffset="65" VerticalOffset="-32" AllowsTransparency="True" 
                  IsOpen="{Binding ElementName=vizKey2, Path=IsChecked, Converter={cnv:UniConverter InvertValue=False}, UpdateSourceTrigger=PropertyChanged, Delay=2220}">
  <Grid MinWidth="80" MinHeight="80" x:Name="vizTrg2b" RenderTransformOrigin="0.5,0.5">
    <Grid.Style>
      <Style TargetType="Grid">
        <Style.Triggers>
          <Trigger Property="IsVisible" Value="True">
            <Trigger.EnterActions>
              <BeginStoryboard>
                <Storyboard>
                  <DoubleAnimation Storyboard.TargetProperty="RenderTransform.Children[0].ScaleX" To="1.0" Duration="0:0:0.33" BeginTime="0:0:.125" EasingFunction="{StaticResource ElasticEaseOut_0}"/>
                  <DoubleAnimation Storyboard.TargetProperty="RenderTransform.Children[0].ScaleY" To="1.0" Duration="0:0:0.33" BeginTime="0:0:.125" EasingFunction="{StaticResource ElasticEaseOut_0}"/>
                </Storyboard>
              </BeginStoryboard>
            </Trigger.EnterActions>
            <Trigger.ExitActions>
              <BeginStoryboard>
                <Storyboard>
                  <DoubleAnimation Storyboard.TargetProperty="RenderTransform.Children[0].ScaleX" To="0.0" Duration="0:0:0.01" BeginTime="0:0:.005" EasingFunction="{StaticResource ElasticEaseOut_0}"/>
                </Storyboard>
              </BeginStoryboard>
            </Trigger.ExitActions>
          </Trigger>
        </Style.Triggers>
      </Style>
    </Grid.Style>
    <Grid.RenderTransform>
      <TransformGroup>
        <ScaleTransform x:Name="PopupScaleTransform" ScaleX="0.0" ScaleY="0.0" />
        <SkewTransform/>
        <SkewTransform/>
        <RotateTransform Angle="0"/>
        <TranslateTransform/>
      </TransformGroup>
    </Grid.RenderTransform>
    <Border CornerRadius="26" BorderThickness="1" BorderBrush="#555" Background="#3888" MouseDown="DragHandle_MouseDown" MouseMove="DragHandle_MouseMove" MouseUp="DragHandle_MouseUp" Cursor="Hand" ToolTip="Use the mouse to drag this window around">
      <Border.Style>
        <Style TargetType="{x:Type Border}">
          <Setter Property="Opacity" Value="0.01"/>
          <Style.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
              <Setter Property="Opacity" Value="1"/>
            </Trigger>
          </Style.Triggers>
        </Style>
      </Border.Style>
    </Border>
    <Border Style="{DynamicResource BorderStyle_Aav0}" Margin="32" VerticalAlignment="Top" HorizontalAlignment="Right" MinWidth="90" MinHeight="60" Opacity=".888" Background="{StaticResource BackBackgroundBrush}">
      <MiscViews:ResultsReportView />
    </Border>
    <ToggleButton Grid.Row="1" Content="r" FontFamily="Webdings" FontSize="19" Width="32" Height="32" Margin="32" VerticalAlignment="Top" HorizontalAlignment="Right" x:Name="tbA" IsChecked="{Binding IsChecked, Converter={cnv:UniConverter InvertValue=True}, ElementName=vizKey2, FallbackValue=True, Mode=Default}"  ToolTip="Close popup" Background="Transparent" Foreground="Red" />
  </Grid>
</Popup>

    */

    void onWindowMinimize(object s, RoutedEventArgs e) => WindowState = System.Windows.WindowState.Minimized;
    void onWindowRestoree(object s, RoutedEventArgs e) { wr.Visibility = Visibility.Collapsed; wm.Visibility = Visibility.Visible; WindowState = System.Windows.WindowState.Normal; }
    void onWindowMaximize(object s, RoutedEventArgs e) { wm.Visibility = Visibility.Collapsed; wr.Visibility = Visibility.Visible; WindowState = System.Windows.WindowState.Maximized; }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
      const string accelkeys = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ`~!@#$%^&*()_+";
      int i = 0;
      var rr = LessonHelper.LoadDrillDataArray();
      foreach (var dr in rr)
        mniFileDrills.Items.Add(new MenuItem
        {
          Header = $"_{accelkeys[i++]} \t{dr.Header}",
          CommandParameter = dr.SqlExcerciseName,
          //Command = .. used through Style.
        });

      t2.Focus();
    }
  }
}