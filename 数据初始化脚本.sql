truncate table DownDataStatus
truncate table StockHistoryData
truncate table StockIndex
truncate table StockIndexTemplate
truncate table StockList
truncate table StockMatchedIndex

insert into StockList(StockCode,StockName) values('000001.ss','上证指数')
insert into StockList(StockCode,StockName) values('399001.sz','上证指数')

insert into StockIndex(StockIndexID,StockIndexName) values(1,'Suspended')
insert into StockIndex(StockIndexID,StockIndexName) values(2,'Event')

insert into StockIndex(StockIndexID,StockIndexName) values(100,'le_non')
insert into StockIndex(StockIndexID,StockIndexName) values(101,'le_down')
insert into StockIndex(StockIndexID,StockIndexName) values(102,'le_ddown')
insert into StockIndex(StockIndexID,StockIndexName) values(103,'le_up')
insert into StockIndex(StockIndexID,StockIndexName) values(104,'le_dup')

insert into StockIndex(StockIndexID,StockIndexName) values(200,'lk_non')
insert into StockIndex(StockIndexID,StockIndexName) values(201,'lk_hammer')
insert into StockIndex(StockIndexID,StockIndexName) values(202,'lk_fallHammer')
insert into StockIndex(StockIndexID,StockIndexName) values(203,'lk_cross')
insert into StockIndex(StockIndexID,StockIndexName) values(204,'lk_changYang')
insert into StockIndex(StockIndexID,StockIndexName) values(205,'lk_changYin')
insert into StockIndex(StockIndexID,StockIndexName) values(211,'lk_positionLow')
insert into StockIndex(StockIndexID,StockIndexName) values(212,'lk_positionHigh')


insert into StockIndex(StockIndexID,StockIndexName) values(300,'kdj_non')
insert into StockIndex(StockIndexID,StockIndexName) values(301,'kdj_overSell')
insert into StockIndex(StockIndexID,StockIndexName) values(302,'kdj_overBuy')
insert into StockIndex(StockIndexID,StockIndexName) values(311,'kdjl_up')
insert into StockIndex(StockIndexID,StockIndexName) values(312,'kdjl_down')

insert into StockIndex(StockIndexID,StockIndexName) values(400,'vr_non')
insert into StockIndex(StockIndexID,StockIndexName) values(401,'vr_Low')
insert into StockIndex(StockIndexID,StockIndexName) values(402,'vr_Normal')
insert into StockIndex(StockIndexID,StockIndexName) values(403,'vr_Rise')
insert into StockIndex(StockIndexID,StockIndexName) values(404,'vr_High')
insert into StockIndex(StockIndexID,StockIndexName) values(405,'vr_Hight')

insert into StockIndex(StockIndexID,StockIndexName) values(500,'wr_non')
insert into StockIndex(StockIndexID,StockIndexName) values(501,'wr_overSell')
insert into StockIndex(StockIndexID,StockIndexName) values(502,'wr_overBuy')

insert into StockIndex(StockIndexID,StockIndexName) values(600,'psy_non')
insert into StockIndex(StockIndexID,StockIndexName) values(601,'psy_low')
insert into StockIndex(StockIndexID,StockIndexName) values(602,'psy_high')
insert into StockIndex(StockIndexID,StockIndexName) values(603,'psy_lower')
insert into StockIndex(StockIndexID,StockIndexName) values(604,'psy_higher')
insert into StockIndex(StockIndexID,StockIndexName) values(605,'psy_lowest')
insert into StockIndex(StockIndexID,StockIndexName) values(606,'psy_highest')



