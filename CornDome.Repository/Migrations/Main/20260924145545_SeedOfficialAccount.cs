using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main
{
    /// <inheritdoc />
    public partial class SeedOfficialAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO
                    User (Id, UserName, Email)
                VALUES
                    (0, 'Official Deck Lists', '');

                UPDATE sqlite_sequence
                SET seq = 999
                WHERE name = 'Decks';

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1000, 0, '22;0:1,1:1,2:1,3:1;24:2,27:2,32:2,34:2,38:1,42:2,45:2,46:2,47:2,48:2,50:1,52:1,53:2,54:2,55:2,59:2,61:2,62:1,63:2,64:1,65:2,66:1,68:2', 22, 'Finn Collectors Pack Deck', DATE(), DATE(), 0, 'Finn Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1001, 0, '23;4:1,5:1,6:1,7:1;25:1,26:2,28:1,29:1,30:2,31:2,33:1,35:1,36:1,37:2,39:2,40:2,41:2,43:2,44:1,49:2,51:2,56:2,57:1,58:2,60:2,61:2,64:2,65:1,67:1', 23, 'Jake Collectors Pack Deck', DATE(), DATE(), 0, 'Jake Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1002, 0, '69;8:1,9:1,10:1,11:1;64:2,65:1,72:1,73:2,74:1,75:1,76:2,78:2,80:2,81:1,82:2,83:2,85:2,86:2,89:2,97:1,99:1,101:2,102:1,103:2,104:2,105:2,106:2,110:1,112:1', 69, 'BMO Collectors Pack Deck', DATE(), DATE(), 0, 'BMO Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1003, 0, '70;12:1,13:1,14:1,15:1;55:2,61:2,64:2,65:1,71:2,77:1,79:2,84:1,87:1,88:2,90:2,91:2,92:1,93:2,94:2,95:2,96:2,98:2,100:2,107:1,108:2,109:2,111:2', 70, 'Lady Rainicorn Collectors Pack Deck', DATE(), DATE(), 0, 'Lady Rainicorn Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1004, 0, '114;16:1,17:1,567:1,568:1;64:1,65:1,115:2,116:2,117:1,118:2,119:2,122:2,123:2,124:2,127:1,135:1,136:2,137:2,139:2,141:2,145:2,146:2,148:1,149:2,151:2,152:2,157:1,161:1', 114, 'Princess Bubblegum Collectors Pack Deck', DATE(), DATE(), 0, 'Princess Bubblegum Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1005, 0, '113;0:1,1:1,8:1,9:1;64:1,120:2,121:2,125:2,126:2,128:2,129:2,130:2,131:2,132:1,133:2,134:2,138:1,140:1,142:2,143:2,144:1,147:2,150:1,153:1,154:2,155:1,156:1,158:1,159:1,160:1', 113, 'Lumpy Space Princess Collectors Pack Deck', DATE(), DATE(), 0, 'Lumpy Space Princess Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1006, 0, '162;18:1,19:1,20:1,21:1;42:2,65:1,164:2,167:1,171:2,174:2,176:2,177:2,181:2,185:2,186:2,187:1,188:2,189:2,190:2,191:1,197:2,198:2,199:2,200:2,202:1,205:1,207:1,208:1', 162, 'Ice King Collectors Pack Deck', DATE(), DATE(), 0, 'Ice King Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1007, 0, '163;8:1,9:1,16:1,17:1;165:2,166:1,168:1,169:2,170:1,172:2,173:2,175:2,178:2,179:2,180:2,182:2,183:2,184:2,192:2,193:2,194:2,195:1,196:2,201:2,203:1,204:1,206:1,209:1', 163, 'Marceline Collectors Pack Deck', DATE(), DATE(), 0, 'Marceline Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1008, 0, '210;4:1,5:1,12:1,13:1;212:1,213:1,214:2,215:1,216:2,217:2,218:2,219:1,220:1,221:2,222:1,223:1,224:2,225:2,226:2,241:1,242:1,243:2,244:2,245:1,246:2,247:1,248:1,255:2,257:1,258:1,259:1,264:1', 210, 'Lemongrab Collectors Pack Deck', DATE(), DATE(), 0, 'Lemongrab Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1009, 0, '211;0:1,1:1,18:1,19:1;227:2,228:1,229:1,230:1,231:2,232:2,233:2,234:2,235:2,236:2,237:2,238:2,239:2,240:2,249:2,250:2,251:1,252:2,253:2,254:1,256:1,260:1,261:1,262:1,263:1', 211, 'Gunter Collectors Pack Deck', DATE(), DATE(), 0, 'Gunter Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1010, 0, '265;0:1,1:1,2:1,3:1;267:2,268:2,269:2,270:2,271:1,272:2,273:1,274:1,275:2,276:1,277:2,278:2,279:2,280:2,296:2,297:2,298:2,299:2,300:2,301:1,302:1,308:1,309:1,313:2', 265, 'Fionna Collectors Pack Deck', DATE(), DATE(), 0, 'Fionna Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1011, 0, '266;4:1,5:1,6:1,7:1;281:2,282:2,283:2,284:2,285:2,286:2,287:1,288:1,289:2,290:2,291:1,292:2,293:2,294:2,295:2,303:1,304:2,305:2,306:1,307:1,310:2,311:1,312:1,314:2', 266, 'Cake Collectors Pack Deck', DATE(), DATE(), 0, 'Cake Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1012, 0, '315;4:1,5:1,16:1,17:1;335:1,345:2,346:2,347:1,348:1,349:2,350:1,351:1,352:2,365:1,366:2,367:1,369:2,370:2,372:2,374:1,376:1,381:1,382:1,383:1,384:1,395:1,396:1,401:1,403:1,409:1,411:1,413:1,421:2,422:1,426:1', 315, 'Jake Doubles Tournament Deck', DATE(), DATE(), 0, 'Jake Doubles Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1013, 0, '316;0:1,1:1,16:1,17:1;336:1,337:1,338:2,339:1,340:2,341:1,342:2,343:2,344:2,365:2,367:1,368:2,369:2,371:1,372:1,373:1,375:2,385:1,386:1,387:1,393:1,394:1,402:1,404:1,408:1,412:1,414:1,419:1,420:2,425:1', 316, 'Charlie Doubles Tournament Deck', DATE(), DATE(), 0, 'Charlie Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1014, 0, '317;12:1,13:1,18:1,19:1;232:1,327:2,328:2,329:2,330:1,331:1,332:2,333:1,334:1,354:1,356:1,358:1,359:2,360:1,361:2,362:2,363:1,364:1,379:1,380:1,382:1,388:1,389:1,390:1,398:1,399:1,405:1,407:1,413:1,417:2,418:1,424:1', 317, 'Grand Prix Doubles Tournament Deck', DATE(), DATE(), 0, 'Grand Prix Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1015, 0, '318;8:1,9:1,18:1,19:1;232:1,319:2,320:1,321:1,322:1,323:2,324:2,325:2,326:1,353:1,354:1,355:1,357:1,358:2,360:1,361:2,362:1,363:2,377:1,378:1,381:1,387:1,391:1,392:1,397:1,400:1,406:1,410:1,414:1,415:2,416:1,423:1', 318, 'Moniker Doubles Tournament Deck', DATE(), DATE(), 0, 'Moniker Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1016, 0, '606;601:1,602:1,603:1,604:1;582:1,592:2,607:2,608:2,609:2,612:1,613:1,615:2,621:2,632:1,674:2,717:2,721:2,777:2,778:2,779:1,780:1,781:2,782:2,783:2,784:1,785:1,786:2,787:1,788:1', 606, 'Flame Princess Collectors Pack Deck', DATE(), DATE(), 0, 'Flame Princess Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1017, 0, '605;4:1,5:1,16:1,17:1;579:2,580:2,596:2,619:2,623:1,633:2,660:2,661:1,675:2,703:1,714:1,718:2,789:2,790:2,791:1,792:1,793:1,794:2,795:2,797:2,798:1,799:1,800:2,801:1,802:2', 605, 'Fern Collectors Pack Deck', DATE(), DATE(), 0, 'Fern Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1018, 0, '644;8:1,9:1,601:1,602:1;576:2,590:1,635:2,637:2,639:2,663:2,667:2,672:2,673:2,676:1,711:2,722:1,723:1,726:2,727:1,728:2,729:2,730:1,731:1,732:2,734:2,735:2,737:2,738:1', 644, 'Peppermint Butler Collectors Pack Deck', DATE(), DATE(), 0, 'Peppermint Butler Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1019, 0, '634;12:1,13:1,16:1,17:1;587:1,589:2,591:1,593:2,594:2,595:1,611:2,631:2,640:2,651:2,652:1,658:2,668:1,715:2,739:2,740:2,741:1,742:2,743:2,744:2,745:1,746:2,747:1,861:2', 634, 'Magic Man Collectors Pack Deck', DATE(), DATE(), 0, 'Magic Man Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1020, 0, '647;0:1,4:1,12:1,601:1;585:2,599:2,600:1,627:1,643:2,650:2,679:2,681:1,709:1,720:1,725:1,748:2,749:2,750:2,751:1,752:2,753:2,754:1,755:2,756:2,757:2,758:2,759:2,760:1,761:1', 647, 'Prismo Collectors Pack Deck', DATE(), DATE(), 0, 'Prismo Precon (Official)');

                INSERT INTO Decks (Id, UserId, DeckString, IconCardId, Description, Created, Modified, Visibility, DeckName)
                VALUES (1021, 0, '670;8:1,9:1,18:1,19:1;573:2,586:1,610:1,616:2,628:2,653:2,655:2,657:1,716:2,762:2,763:2,764:2,765:2,766:2,767:2,768:2,769:2,770:1,771:1,772:2,773:1,774:1,775:1,776:1,860:1', 670, 'The Lich Collectors Pack Deck', DATE(), DATE(), 0, 'The Lich Precon (Official)');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No going back
        }
    }
}
