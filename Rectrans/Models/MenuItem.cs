using System.Windows.Input;
using System.Collections.ObjectModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.Models;

internal class MenuItem
{
    public string Key { get; set; } = null!;

    public string Header { get; set; } = null!;

    public bool IsCheckable { get; set; }

    public bool IsChecked { get; set; }

    public Group Group { get; set; }

    public ICommand? Command { get; set; }

    public object? CommandParameter { get; set; }

    public bool HasItems => ItemsSource != null && ItemsSource.Any();

    public ObservableCollection<MenuItem>? ItemsSource { get; set; }

    public object? Extra { get; set; }

    public static readonly ObservableCollection<MenuItem> DefaultCollection = new()
    {
        #region DefaultCollection

        new()
        {
            Key = "Language",
            Header = "语言",
            Group = Group.None,
            ItemsSource = new()
            {
                #region SOURE_LAN

                new()
                {
                    Key = "Source",
                    Header = "源语言",
                    Group = Group.Language,
                    ItemsSource = new()
                    {
                        new()
                        {
                            Key = "English",
                            Header = "英语",
                            Group = Group.SourceLan,
                            IsCheckable = true,
                            IsChecked = true,
                            Extra = "en"
                        },
                        new()
                        {
                            Key = "Japanese",
                            Header = "日语",
                            Group = Group.SourceLan,
                            IsCheckable = true,
                            Extra = "ja"
                        },
                        new()
                        {
                            Key = "Korean",
                            Header = "韩语",
                            Group = Group.SourceLan,
                            IsCheckable = true,
                            Extra = "ko"
                        },
                        new()
                        {
                            Key = "ChineseSimplified",
                            Header = "简体中文",
                            Group = Group.SourceLan,
                            IsCheckable = true,
                            Extra = "zh"
                        },
                    }
                },

                #endregion

                #region TARGET_LAN

                new()
                {
                    Key = "Target",
                    Header = "目标语言",
                    Group = Group.Language,
                    ItemsSource = new()
                    {
                        new()
                        {
                            Key = "ChineseSimplified",
                            Header = "简体中文",
                            Group = Group.TargetLan,
                            IsCheckable = true,
                            IsChecked = true,
                            Extra = "zh"
                        },
                        new()
                        {
                            Key = "English",
                            Header = "英语",
                            Group = Group.TargetLan,
                            IsCheckable = true,
                            Extra = "en"
                        },
                        new()
                        {
                            Key = "Japanese",
                            Header = "日语",
                            Group = Group.TargetLan,
                            IsCheckable = true,
                            Extra = "ja"
                        },
                        new()
                        {
                            Key = "Korean",
                            Header = "韩语",
                            Group = Group.TargetLan,
                            IsCheckable = true,
                            Extra = "ko"
                        },
                    }
                },

                #endregion

            }
        },
        new()
        {
            Key = "AutoTranslate",
            Header = "自动翻译",
            Group = Group.None,
            ItemsSource = new()
            {
                #region AUTO_TRANSLATE
                new()
                {
                    Header = "停止",
                    Group = Group.AntoTranslate
                },
                new()
                {
                    Header = "2s",
                    Group = Group.AntoTranslate,
                    IsCheckable = true,
                    Extra = 2000
                },
                new()
                {
                    Header = "4s",
                    Group = Group.AntoTranslate,
                    IsCheckable = true,
                    Extra = 4000
                },
                new()
                {
                    Header = "8s",
                    Group = Group.AntoTranslate,
                    IsCheckable = true,
                    Extra = 8000
                },
                new()
                {
                    Header = "12s",
                    Group = Group.AntoTranslate,
                    IsCheckable = true,
                    Extra = 12000
                },
                #endregion
            }
        }

        #endregion
    };
}