# Meditation Count bot

This is a telegram bot [https://t.me/MeditationCountBot](https://t.me/MeditationCountBot) designed to count the meditation time in a telegram group 

If you want to use this bot, you need to add it to the group and give it the rights to read and send messages

The band members write their meditation time in minutes in the format
+15
+20
+60

The bot calculates the total meditation time and at the end of the day sends the total meditation time, as well as the number of days each group member meditates without a break in the format

They meditate for 10 days in a row:
- Participant 1
- Participant 2
- etc.

Meditate for 5 days in a row:
- Participant 3
- Participant 4
- etc.

Meditate for 2 days in a row:
- Participant 5
- etc.

For read counter json-files and update on memory dictionary
curl http://localhost:8080/api/main/reload
For update todays calculations and send a result to chats (send message only if time now is 11 pm)
curl http://localhost:8080/api/main/message?mode=prod