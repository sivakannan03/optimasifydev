<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:template match="/"> 
		<div id="tableContainer">
			<table  align="left" bgcolor="white" cellspacing="1" cellpadding="0"  class="inner_tb"   border="1" isHeader="1" id="tblBrands" consider="1" startRow="1" parameter="DSARHeader">
				<!-- New Code -->	
				<tr class="tableheader" >
							<th colspan="15" align="center" nowrap="nowrap" >Daily Report at a glance (Regular) : <xsl:value-of select="//row[@Element='HDate']/@HeaderDate" /></th>
						<!--<td colspan="5" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='HDate']/@HeaderDate" />
						</td>-->
				</tr>
				
				<tr class="tableheader" >
						<th colspan="2" align="right" nowrap="nowrap" >User Name : </th>
						<td colspan="2" align="left" nowrap="nowrap">
              <b>
							<xsl:value-of select="//row[@Element='UName']/@UserName" /></b>
						</td>
						<td colspan="2" align="right" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='ActType']/@VisitType" />
						</td>
						<td colspan="4" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='FDate']/@FetchDate" />
						</td>
						<th colspan="2" align="right" nowrap="nowrap">Submitted On:</th>
						<td colspan="3" align="right" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='SDate']/@SubmittedDate" />
						</td>
				
				</tr>
				<!-- End -->
		
			<tr class="tableheader" align="center">
					<th>MCL Doctor Name</th>
				  <th>Specialty</th>					
					<th >Brick</th>
					<th  >Focus Doctor</th>
					<th  nowrap="nowrap">RR-CP</th>
					<th  >CP Products</th>
					<th >Call</th>										
					<th >Plan</th>
					<th  nowrap="nowrap">RR-DR</th>
					<th  >DR Product</th>
					<th  >PCL Doctors</th>
					<th >MCL Chemist </th>
					<th  >Call </th>
					<th  >PCL Chemist  </th>
					<th  >Joint working with</th>
			</tr>
				 
				<xsl:choose>
					<xsl:when test="count(//row[@Element='DRPSO']/@PSOFKID) &gt;0" >		
						<xsl:apply-templates select="//row[@Element='DRPSO']"/>
					</xsl:when>
					<xsl:otherwise>
						<tr class="tablecontent1"> 
							<td align="center"  class="tb-r2" colspan="17" >No Records Found !</td>
						</tr>
					</xsl:otherwise>
				</xsl:choose>
			</table>
		</div>
	</xsl:template>

	<xsl:template match="//row[@Element='DRPSO']">
	
		<xsl:variable name="Dr-id" select="@MDrId" />
		
		
		<tr class="tablecontent2">
			<td align="left" class="tb-r1" nowrap="nowrap">
			<xsl:choose>
				<xsl:when test="@NodeType='DMRBM'">
					<xsl:attribute name="class">tablecontentDMRBM</xsl:attribute>
				</xsl:when>	
			</xsl:choose>			
			<xsl:choose>
				<xsl:when test="@NodeType='DM'">
					<xsl:attribute name="class">tablecontentDM</xsl:attribute>
				</xsl:when>	
			</xsl:choose>			
			<xsl:choose>
				<xsl:when test="@NodeType='RBM'">
					<xsl:attribute name="class">tablecontentRBM</xsl:attribute>
				</xsl:when>	
			</xsl:choose>	
							<xsl:value-of select="@MDrName" />		

			</td>
			
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:attribute name="nowrap"/>
				<xsl:value-of select="@Specialty" />
			</td>
			
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:attribute name="nowrap"/>
				<xsl:value-of select="@MDrBrick" />
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:value-of select="@MDrFD" />
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:for-each select ="//row[@Element='CPProd' and @DoctorFKID=$Dr-id]">
				<xsl:attribute name="nowrap"/>
					<xsl:value-of select="@ReminderFlag"></xsl:value-of><br/>
				</xsl:for-each>					
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:for-each select ="//row[@Element='CPProd' and @DoctorFKID=$Dr-id]">
				<xsl:attribute name="nowrap"/>
					<xsl:value-of select="@ProductName"></xsl:value-of><br/>
				</xsl:for-each>					
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:value-of select="@MCDr" />
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:value-of select="@MPDr" />
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:for-each select ="//row[@Element='DRProd' and @DoctorChemistFKID=$Dr-id]">
					<xsl:attribute name="nowrap"/>
					<xsl:value-of select="@ReminderFlag"></xsl:value-of><br/>
				</xsl:for-each>					
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:for-each select ="//row[@Element='DRProd' and @DoctorChemistFKID=$Dr-id]">
					<xsl:attribute name="nowrap"/>
					<xsl:value-of select="@ProductName"></xsl:value-of><br/>
				</xsl:for-each>					
			</td>

			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:value-of select="@PDrName" />
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">

			<xsl:choose>
				<xsl:when test="@ChNodeType='DMRBM'">
					<xsl:attribute name="class">tablecontentDMRBM</xsl:attribute>
				</xsl:when>	
			</xsl:choose>			
			<xsl:choose>
				<xsl:when test="@ChNodeType='DM'">
					<xsl:attribute name="class">tablecontentDM</xsl:attribute>
				</xsl:when>	
			</xsl:choose>			
			<xsl:choose>
				<xsl:when test="@ChNodeType='RBM'">
					<xsl:attribute name="class">tablecontentRBM</xsl:attribute>
				</xsl:when>	
			</xsl:choose>	
							<xsl:value-of select="@MChName" />		
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:value-of select="@MCCh" />
			</td>
			<td align="left" class="tb-r1"  nowrap="nowrap">
				<xsl:value-of select="@PChName" />
			</td>
			<td align="left" class="tb-r1" >
				<xsl:value-of select="@JointWorking" />
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>