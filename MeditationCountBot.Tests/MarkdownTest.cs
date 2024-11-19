using System.Text.Json;
using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using Moq;

namespace MeditationCountBot.Tests;

public class MarkdownTest
{
    private const string jsonCounter =
        """{"ChatId":"-1002065988567","Best":"10:37:00","Yesterday":"06:32:00","Today":"00:52:00","Participants":[{"Id":453949424,"LastName":"akbayev","FirstName":"beibit","Username":null,"Total":"06:15:00","ContinuouslyDays":11,"LastMeditation":"2024-07-29T15:51:30Z"},{"Id":1083580632,"LastName":"Po","FirstName":"Anna","Username":"AnnaPot23","Total":"04:45:00","ContinuouslyDays":5,"LastMeditation":"2024-07-29T08:02:03Z"},{"Id":249696007,"LastName":"Котлярова","FirstName":"Мария","Username":"Sobrilliant","Total":"01:05:00","ContinuouslyDays":0,"LastMeditation":"2024-07-26T19:33:16Z"},{"Id":115601429,"LastName":"Shakhlin","FirstName":"Vitaliy","Username":"vshakhlin","Total":"05:20:00","ContinuouslyDays":11,"LastMeditation":"2024-07-29T16:03:35Z"},{"Id":1038184572,"LastName":null,"FirstName":"Александр","Username":null,"Total":"03:10:00","ContinuouslyDays":0,"LastMeditation":"2024-07-28T10:44:22Z"},{"Id":832931269,"LastName":"Kovalenko","FirstName":"Marina","Username":"marina_yogina","Total":"03:50:00","ContinuouslyDays":0,"LastMeditation":"2024-07-27T23:10:32Z"},{"Id":1058269385,"LastName":"Tikhomirova","FirstName":"Anna","Username":"Anna_A_Tikhomirova","Total":"08:20:00","ContinuouslyDays":11,"LastMeditation":"2024-07-29T22:53:37Z"},{"Id":5251685616,"LastName":"Логинова","FirstName":"Валерия","Username":"valeria_chakras","Total":"06:20:00","ContinuouslyDays":11,"LastMeditation":"2024-07-29T07:23:08Z"},{"Id":714543091,"LastName":null,"FirstName":"Вика","Username":"VIKA_WBH","Total":"03:52:00","ContinuouslyDays":11,"LastMeditation":"2024-07-30T04:35:17Z"},{"Id":264266748,"LastName":null,"FirstName":"Генрих","Username":"GenriZak","Total":"07:08:00","ContinuouslyDays":10,"LastMeditation":"2024-07-29T06:58:34Z"},{"Id":163305817,"LastName":null,"FirstName":"Alexander Grekov","Username":"sprtrmp","Total":"03:00:00","ContinuouslyDays":11,"LastMeditation":"2024-07-30T05:33:10Z"},{"Id":395494598,"LastName":"Dubovsky","FirstName":"Еvgeny","Username":"Evgenweb","Total":"03:55:00","ContinuouslyDays":11,"LastMeditation":"2024-07-29T19:26:57Z"},{"Id":332736019,"LastName":"Bai","FirstName":"Masha","Username":"Baichi91","Total":"03:13:00","ContinuouslyDays":2,"LastMeditation":"2024-07-29T22:28:31Z"},{"Id":288455543,"LastName":"Vinogradov","FirstName":"Artem","Username":"artvinoand","Total":"02:21:00","ContinuouslyDays":0,"LastMeditation":"2024-07-25T09:23:23Z"},{"Id":1261978297,"LastName":"Берюх +7","FirstName":"Екатерина","Username":"ekaterina_beryuh","Total":"05:30:00","ContinuouslyDays":11,"LastMeditation":"2024-07-29T15:32:42Z"},{"Id":869119233,"LastName":null,"FirstName":"Alena","Username":"A_lenka_0201","Total":"03:05:00","ContinuouslyDays":2,"LastMeditation":"2024-07-29T09:25:16Z"},{"Id":5156676814,"LastName":null,"FirstName":"Лариса","Username":null,"Total":"00:20:00","ContinuouslyDays":0,"LastMeditation":"2024-07-23T07:44:06Z"},{"Id":717760566,"LastName":null,"FirstName":"Садыков | Альберт","Username":"Sadykov_Albert","Total":"04:10:00","ContinuouslyDays":5,"LastMeditation":"2024-07-30T05:37:27Z"},{"Id":751290128,"LastName":"Назырова","FirstName":"Дилара","Username":"dilara_nazyrova","Total":"01:35:00","ContinuouslyDays":1,"LastMeditation":"2024-07-29T21:19:05Z"},{"Id":324959554,"LastName":null,"FirstName":"Sergei","Username":"Narbelas","Total":"03:15:00","ContinuouslyDays":0,"LastMeditation":"2024-07-27T04:05:12Z"},{"Id":482241343,"LastName":null,"FirstName":"Asya","Username":null,"Total":"00:25:00","ContinuouslyDays":0,"LastMeditation":"2024-07-24T20:02:08Z"},{"Id":516487324,"LastName":null,"FirstName":"anna.prudnikovaa","Username":"blago_dat","Total":"00:40:00","ContinuouslyDays":0,"LastMeditation":"2024-07-25T16:09:19Z"},{"Id":358069117,"LastName":"Turunkina","FirstName":"Elena","Username":"eturunkina","Total":"02:21:00","ContinuouslyDays":1,"LastMeditation":"2024-07-29T07:59:26Z"},{"Id":605896437,"LastName":null,"FirstName":"Дмитрий","Username":"Dimonsl","Total":"00:40:00","ContinuouslyDays":0,"LastMeditation":"2024-07-28T05:29:28Z"},{"Id":147547823,"LastName":null,"FirstName":"Артур","Username":"wordbk","Total":"00:15:00","ContinuouslyDays":0,"LastMeditation":"2024-07-27T10:04:14Z"},{"Id":550846383,"LastName":null,"FirstName":"Vasily","Username":"Vasily_Zvyagin","Total":"01:06:00","ContinuouslyDays":2,"LastMeditation":"2024-07-29T06:58:10Z"}]}""";
    
    [Fact]
    public void EscapeUsernameTest()
    {
        var counter = JsonSerializer.Deserialize<CounterDto>(jsonCounter);
        var messageFormer = new MessageFormer(new TimeFormatter(), Mock.Of<IPraiseAndCheerMessage>());
        var message = messageFormer.CreateMessage(counter);
        Assert.NotNull(message);
        Assert.True(message.Contains("""Садыков \| Альберт \(@Sadykov\_Albert\)"""));
        Assert.True(message.Contains("""Екатерина Берюх \+7 \(@ekaterina\_beryuh\)"""));
        Assert.True(message.Contains("""Вика \(@VIKA\_WBH\)"""));
        Assert.True(message.Contains("""Anna Tikhomirova \(@Anna\_A\_Tikhomirova\)"""));
        Assert.True(message.Contains("""Валерия Логинова \(@valeria\_chakras\)"""));
    }
    
    [Fact]
    public void EscapeTest()
    {
        var input = "text, *text*, text, text, (text), [text], [some_text with _*](http://.....) *text*, text, text, (text), [text]";
        var escape = MarkdownHelper.Escape(input);
        Assert.Equal(escape, @"text, \*text\*, text, text, \(text\), \[text\], \[some\_text with \_\*\]\(http://\.\.\.\.\.\) \*text\*, text, text, \(text\), \[text\]");
    }
    
    
}