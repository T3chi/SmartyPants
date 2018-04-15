using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextAnalyzer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void UserInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SentenceCount.Text = "Sentences:" + TextStatistics.Net.TextStatistics.Parse(UserInputTextBox.Text).SentenceCount;
            //SentenceCount.Text = "Sentences: "+ScoreFinder.SentenceCount(UserInputTextBox.Text).ToString();
            WordCount.Text = "Words: " + TextStatistics.Net.TextStatistics.Parse(UserInputTextBox.Text).WordCount;
            //WordCount.Text = "Words: "+ScoreFinder.WordCount(UserInputTextBox.Text).ToString();
            SyllableCount.Text = "Syllables: "+ScoreFinder.NumberOfSyllablesInString(UserInputTextBox.Text).ToString();

            Score.Text = "Flesch-Kincaid: " + TextStatistics.Net.TextStatistics.Parse(UserInputTextBox.Text).FleschKincaidReadingEase();
            double score = TextStatistics.Net.TextStatistics.Parse(UserInputTextBox.Text).FleschKincaidReadingEase();
            //Score.Text ="Reading Ease: " + ScoreFinder.CalcScoreFromText(UserInputTextBox.Text).ToString();
            if (score > 100)
            {
                GradeScore.Text = "Reading Level: < 5th grade";
            }
            else
            {
                if (score < 100 && score >= 90)
                    GradeScore.Text = "Reading Level: 5th grade";
                if (score < 90 && score >= 80)
                    GradeScore.Text = "Reading Level: 6th grade";
                else if (score < 80 && score >= 70)
                    GradeScore.Text = "Reading Level: 7th grade";
                else if (score < 70 && score >= 60)
                    GradeScore.Text = "Reading Level: 8-9th grade";
                else if (score < 60 && score >= 50)
                    GradeScore.Text = "Reading Level: 10-12th grade";
                else if (score < 50 && score >= 40)
                    GradeScore.Text = "Reading Level: College Undergraduate";
                else if (score < 40 && score >= 30)
                    GradeScore.Text = "Reading Level: College Graduate";
                else if (score < 30){
                    GradeScore.Text = "Reading Level: College Graduate";
                }
                //GradeScore.Text = "Grade Level: " + ScoreFinder.gradeLevelScore(UserInputTextBox.Text).ToString();
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }

    class ScoreFinder {
       static public int SentenceCount(string text) {
            return text.TrimEnd('.').Split('.','!','?').Count();
        }
        static public int WordCount(string text) {
            return text.Trim().Split(' ','-').Count();
        }
        static public int NumberOfSyllablesInWord(String word)
        {
            int syllabeleCount = 0;
            bool lastWasVowel = false;
            string lowerCase = 
            word.ToLower().Replace("ome", "um").Replace("ime", "im").Replace("imn", "imen").Replace("ine", "in").Replace("ely", "ly").Replace("ure", "ur").Replace("ery", "ry");
            for (int n = 0; n < lowerCase.Length; n++)
            {
                if (isVowel(lowerCase[n]))
                {
                    if (!lastWasVowel)
                        syllabeleCount++;
                    lastWasVowel = true;
                }
                else
                {
                    lastWasVowel = false;
                }
            }
            if (lowerCase.EndsWith("ing") || lowerCase.EndsWith("ings"))
            {
                if (lowerCase.Length > 4 && isVowel(lowerCase[lowerCase.Length - 4]))
                    syllabeleCount++;
            }
            if (lowerCase.EndsWith("e") && !lowerCase.EndsWith("le"))
            {
                syllabeleCount--;
            }
            if (lowerCase.EndsWith("es"))
            {
                if (lowerCase.Length > 4 && isVowel(lowerCase[lowerCase.Length - 4]))
                    syllabeleCount--;
            }
            if (lowerCase.EndsWith("e's"))
            {
                if (lowerCase.Length > 5 && isVowel(lowerCase[lowerCase.Length - 5]))
                    syllabeleCount--;
            }
            if (lowerCase.EndsWith("ed") && !lowerCase.EndsWith("ted") && !lowerCase.EndsWith("ded"))
            {
                syllabeleCount--;
            }
            return syllabeleCount > 0 ? syllabeleCount : 1;
        }
        static public int NumberOfSyllablesInString(string text) {
            string[] words = text.Trim().Split(' ','-');
            int counter = 0;
            foreach (string s in words) {
                counter += NumberOfSyllablesInWord(s);
            }
            return counter;
        }
     static private bool isVowel(char c)
        {
            return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y';
        }
        static public double GetScore(int words,int sentences,int syllables) {
            if (words == 0 || sentences == 0)
                return 0;
            return (206.835 - (1.015 * (words / sentences)) - 84.6 * (syllables / words));
        }
        static public double GetGradeScore(int words, int sentences, int syllables)
        {
           
            
            //FKRA = (0.39 x ASL) +(11.8 x ASW) -15.59
            if (words == 0 || sentences == 0)
            {
                return 0;
            }
            else 
            {
                double ASL = words / sentences;
                double ASW = syllables / words;
                Debug.WriteLine("Calculating: (.39*" + ASL + ") + (11.8*" + ASW + ") -15.59");
                return ((0.39 * ASL) + (11.8 * ASW) - 15.59);
            }
            //return .39*(words/sentences)+11.8*(syllables/words)-15.59;
        }
        static public double CalcScoreFromText(string text) {
            return
                GetScore(WordCount(text),SentenceCount(text),NumberOfSyllablesInString(text));
        }
        static public double gradeLevelScore(string text) {
            //FKRA = (0.39 x ASL) + (11.8 x ASW) - 15.59
            return
                GetGradeScore(WordCount(text), SentenceCount(text), NumberOfSyllablesInString(text));
        }
    }
}
